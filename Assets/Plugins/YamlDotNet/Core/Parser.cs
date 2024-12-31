//  This file is part of YamlDotNet - A .NET library for YAML.
//  Copyright (c) Antoine Aubry and contributors

//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//  of the Software, and to permit persons to whom the Software is furnished to do
//  so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YamlDotNet.Core.Tokens;
using MappingStyle = YamlDotNet.Core.Events.MappingStyle;
using ParsingEvent = YamlDotNet.Core.Events.ParsingEvent;
using SequenceStyle = YamlDotNet.Core.Events.SequenceStyle;

namespace YamlDotNet.Core
{
    /// <summary>
    /// Parses YAML streams.
    /// </summary>
    public class Parser : IParser
    {
        private readonly Stack<ParserState> states = new Stack<ParserState>();
        private readonly TagDirectiveCollection tagDirectives = new TagDirectiveCollection();
        private ParserState state;

        private readonly IScanner scanner;
        private ParsingEvent currentEvent;

        private Token currentToken;

        private Token GetCurrentToken()
        {
            if (currentToken == null)
            {
                while (scanner.MoveNextWithoutConsuming())
                {
                    currentToken = scanner.Current;

                    var commentToken = currentToken as Comment;
                    if (commentToken != null)
                    {
                        pendingEvents.Enqueue(new Events.Comment(commentToken.Value, commentToken.IsInline, commentToken.Start, commentToken.End));
                        scanner.ConsumeCurrent();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return currentToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="input">The input where the YAML stream is to be read.</param>
        public Parser(TextReader input)
            : this(new Scanner(input))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        public Parser(IScanner scanner)
        {
            this.scanner = scanner;
        }

        /// <summary>
        /// Gets the current event.
        /// </summary>
        public ParsingEvent Current
        {
            get
            {
                return currentEvent;
            }
        }

        private readonly EventQueue pendingEvents = new EventQueue();

        /// <summary>
        /// Moves to the next event.
        /// </summary>
        /// <returns>Returns true if there are more events available, otherwise returns false.</returns>
        public bool MoveNext()
        {
            // No events after the end of the stream or error.
            if (state == ParserState.StreamEnd)
            {
                currentEvent = null;
                return false;
            }
            else if (pendingEvents.Count == 0)
            {
                // Generate the next event.
                pendingEvents.Enqueue(StateMachine());
            }

            currentEvent = pendingEvents.Dequeue();
            return true;
        }

        private ParsingEvent StateMachine()
        {
            switch (state)
            {
                case ParserState.StreamStart:
                    return ParseStreamStart();

                case ParserState.ImplicitDocumentStart:
                    return ParseDocumentStart(true);

                case ParserState.DocumentStart:
                    return ParseDocumentStart(false);

                case ParserState.DocumentContent:
                    return ParseDocumentContent();

                case ParserState.DocumentEnd:
                    return ParseDocumentEnd();

                case ParserState.BlockNode:
                    return ParseNode(true, false);

                case ParserState.BlockNodeOrIndentlessSequence:
                    return ParseNode(true, true);

                case ParserState.FlowNode:
                    return ParseNode(false, false);

                case ParserState.BlockSequenceFirstEntry:
                    return ParseBlockSequenceEntry(true);

                case ParserState.BlockSequenceEntry:
                    return ParseBlockSequenceEntry(false);

                case ParserState.IndentlessSequenceEntry:
                    return ParseIndentlessSequenceEntry();

                case ParserState.BlockMappingFirstKey:
                    return ParseBlockMappingKey(true);

                case ParserState.BlockMappingKey:
                    return ParseBlockMappingKey(false);

                case ParserState.BlockMappingValue:
                    return ParseBlockMappingValue();

                case ParserState.FlowSequenceFirstEntry:
                    return ParseFlowSequenceEntry(true);

                case ParserState.FlowSequenceEntry:
                    return ParseFlowSequenceEntry(false);

                case ParserState.FlowSequenceEntryMappingKey:
                    return ParseFlowSequenceEntryMappingKey();

                case ParserState.FlowSequenceEntryMappingValue:
                    return ParseFlowSequenceEntryMappingValue();

                case ParserState.FlowSequenceEntryMappingEnd:
                    return ParseFlowSequenceEntryMappingEnd();

                case ParserState.FlowMappingFirstKey:
                    return ParseFlowMappingKey(true);

                case ParserState.FlowMappingKey:
                    return ParseFlowMappingKey(false);

                case ParserState.FlowMappingValue:
                    return ParseFlowMappingValue(false);

                case ParserState.FlowMappingEmptyValue:
                    return ParseFlowMappingValue(true);

                default:
                    Debug.Assert(false, "Invalid state");      // Invalid state.
                    throw new InvalidOperationException();
            }
        }

        private void Skip()
        {
            if (currentToken != null)
            {
                currentToken = null;
                scanner.ConsumeCurrent();
            }
        }

        /// <summary>
        /// Parse the production:
        /// stream   ::= STREAM-START implicit_document? explicit_document* STREAM-END
        ///              ************
        /// </summary>
        private ParsingEvent ParseStreamStart()
        {
            var streamStart = GetCurrentToken() as StreamStart;
            if (streamStart == null)
            {
                var current = GetCurrentToken();
                throw new SemanticErrorException(current.Start, current.End, "Did not find expected <stream-start>.");
            }
            Skip();

            state = ParserState.ImplicitDocumentStart;
            return new Events.StreamStart(streamStart.Start, streamStart.End);
        }

        /// <summary>
        /// Parse the productions:
        /// implicit_document    ::= block_node DOCUMENT-END*
        ///                          *
        /// explicit_document    ::= DIRECTIVE* DOCUMENT-START block_node? DOCUMENT-END*
        ///                          *************************
        /// </summary>
        private ParsingEvent ParseDocumentStart(bool isImplicit)
        {
            // Parse extra document end indicators.

            if (!isImplicit)
            {
                while (GetCurrentToken() is DocumentEnd)
                {
                    Skip();
                }
            }

            if (GetCurrentToken() is Scalar && (state == ParserState.ImplicitDocumentStart || state == ParserState.DocumentStart))
            {
                isImplicit = true;
            }

            // Parse an isImplicit document.

            if (isImplicit && !(GetCurrentToken() is VersionDirective || GetCurrentToken() is TagDirective || GetCurrentToken() is DocumentStart || GetCurrentToken() is StreamEnd || GetCurrentToken() is DocumentEnd) || GetCurrentToken() is BlockMappingStart)
            {
                var directives = new TagDirectiveCollection();
                ProcessDirectives(directives);

                states.Push(ParserState.DocumentEnd);

                state = ParserState.BlockNode;

                return new Events.DocumentStart(null, directives, true, GetCurrentToken().Start, GetCurrentToken().End);
            }

            // Parse an explicit document.

            else if (!(GetCurrentToken() is StreamEnd || GetCurrentToken() is DocumentEnd))
            {
                Mark start = GetCurrentToken().Start;
                var directives = new TagDirectiveCollection();
                var versionDirective = ProcessDirectives(directives);

                var current = GetCurrentToken();
                if (!(current is DocumentStart))
                {
                    throw new SemanticErrorException(current.Start, current.End, "Did not find expected <document start>.");
                }

                states.Push(ParserState.DocumentEnd);

                state = ParserState.DocumentContent;

                ParsingEvent evt = new Events.DocumentStart(versionDirective, directives, false, start, current.End);
                Skip();
                return evt;
            }

            // Parse the stream end.

            else
            {
                if (GetCurrentToken() is DocumentEnd)
                {
                    Skip();
                }
                state = ParserState.StreamEnd;

                ParsingEvent evt = new Events.StreamEnd(GetCurrentToken().Start, GetCurrentToken().End);
                // Do not call skip here because that would throw an exception
                if (scanner.MoveNextWithoutConsuming())
                {
                    throw new InvalidOperationException("The scanner should contain no more tokens.");
                }
                return evt;
            }
        }

        /// <summary>
        /// Parse directives.
        /// </summary>
        private VersionDirective ProcessDirectives(TagDirectiveCollection tags)
        {
            VersionDirective version = null;
            bool hasOwnDirectives = false;

            while (true)
            {
                VersionDirective currentVersion;
                TagDirective tag;

                if ((currentVersion = GetCurrentToken() as VersionDirective) != null)
                {
                    if (version != null)
                    {
                        throw new SemanticErrorException(currentVersion.Start, currentVersion.End, "Found duplicate %YAML directive.");
                    }

                    if (currentVersion.Version.Major != Constants.MajorVersion || currentVersion.Version.Minor > Constants.MinorVersion)
                    {
                        throw new SemanticErrorException(currentVersion.Start, currentVersion.End, "Found incompatible YAML document.");
                    }

                    version = currentVersion;
                    hasOwnDirectives = true;
                }
                else if ((tag = GetCurrentToken() as TagDirective) != null)
                {
                    if (tags.Contains(tag.Handle))
                    {
                        throw new SemanticErrorException(tag.Start, tag.End, "Found duplicate %TAG directive.");
                    }
                    tags.Add(tag);
                    hasOwnDirectives = true;
                }
                else
                {
                    break;
                }

                Skip();
            }

            AddTagDirectives(tags, Constants.DefaultTagDirectives);

            if (hasOwnDirectives)
            {
                tagDirectives.Clear();
            }

            AddTagDirectives(tagDirectives, tags);

            return version;
        }

        private static void AddTagDirectives(TagDirectiveCollection directives, IEnumerable<TagDirective> source)
        {
            foreach (var directive in source)
            {
                if (!directives.Contains(directive))
                {
                    directives.Add(directive);
                }
            }
        }

        /// <summary>
        /// Parse the productions:
        /// explicit_document    ::= DIRECTIVE* DOCUMENT-START block_node? DOCUMENT-END*
        ///                                                    ***********
        /// </summary>
        private ParsingEvent ParseDocumentContent()
        {
            if (
                GetCurrentToken() is VersionDirective ||
                GetCurrentToken() is TagDirective ||
                GetCurrentToken() is DocumentStart ||
                GetCurrentToken() is DocumentEnd ||
                GetCurrentToken() is StreamEnd
            )
            {
                state = states.Pop();
                return ProcessEmptyScalar(scanner.CurrentPosition);
            }
            else
            {
                return ParseNode(true, false);
            }
        }

        /// <summary>
        /// Generate an empty scalar event.
        /// </summary>
        private static ParsingEvent ProcessEmptyScalar(Mark position)
        {
            return new Events.Scalar(null, null, string.Empty, ScalarStyle.Plain, true, false, position, position);
        }

        /// <summary>
        /// Parse the productions:
        /// block_node_or_indentless_sequence    ::=
        ///                          ALIAS
        ///                          *****
        ///                          | properties (block_content | indentless_block_sequence)?
        ///                            **********  *
        ///                          | block_content | indentless_block_sequence
        ///                            *
        /// block_node           ::= ALIAS
        ///                          *****
        ///                          | properties block_content?
        ///                            ********** *
        ///                          | block_content
        ///                            *
        /// flow_node            ::= ALIAS
        ///                          *****
        ///                          | properties flow_content?
        ///                            ********** *
        ///                          | flow_content
        ///                            *
        /// properties           ::= TAG ANCHOR? | ANCHOR TAG?
        ///                          *************************
        /// block_content        ::= block_collection | flow_collection | SCALAR
        ///                                                               ******
        /// flow_content         ::= flow_collection | SCALAR
        ///                                            ******
        /// </summary>
        private ParsingEvent ParseNode(bool isBlock, bool isIndentlessSequence)
        {
            var alias = GetCurrentToken() as AnchorAlias;
            if (alias != null)
            {
                state = states.Pop();
                ParsingEvent evt = new Events.AnchorAlias(alias.Value, alias.Start, alias.End);
                Skip();
                return evt;
            }

            Mark start = GetCurrentToken().Start;

            Anchor anchor = null;
            Tag tag = null;

            // The anchor and the tag can be in any order. This loop repeats at most twice.
            while (true)
            {
                if (anchor == null && (anchor = GetCurrentToken() as Anchor) != null)
                {
                    Skip();
                }
                else if (tag == null && (tag = GetCurrentToken() as Tag) != null)
                {
                    Skip();
                }
                else if (GetCurrentToken() is AnchorAlias anchorAlias)
                {
                    throw new SemanticErrorException(anchorAlias.Start, anchorAlias.End, "While parsing a node, did not find expected token.");
                }
                else
                {
                    break;
                }
            }

            string tagName = null;
            if (tag != null)
            {
                if (string.IsNullOrEmpty(tag.Handle))
                {
                    tagName = tag.Suffix;
                }
                else if (tagDirectives.Contains(tag.Handle))
                {
                    tagName = string.Concat(tagDirectives[tag.Handle].Prefix, tag.Suffix);
                }
                else
                {
                    throw new SemanticErrorException(tag.Start, tag.End, "While parsing a node, found undefined tag handle.");
                }
            }
            if (string.IsNullOrEmpty(tagName))
            {
                tagName = null;
            }

            string anchorName = anchor != null ? string.IsNullOrEmpty(anchor.Value) ? null : anchor.Value : null;

            var isImplicit = string.IsNullOrEmpty(tagName);

            if (isIndentlessSequence && GetCurrentToken() is BlockEntry)
            {
                state = ParserState.IndentlessSequenceEntry;

                return new Events.SequenceStart(
                           anchorName,
                           tagName,
                           isImplicit,
                           SequenceStyle.Block,
                           start,
                           GetCurrentToken().End
                       );
            }
            else
            {
                var scalar = GetCurrentToken() as Scalar;
                if (scalar != null)
                {
                    bool isPlainImplicit = false;
                    bool isQuotedImplicit = false;
                    if ((scalar.Style == ScalarStyle.Plain && tagName == null) || tagName == Constants.DefaultHandle)
                    {
                        isPlainImplicit = true;
                    }
                    else if (tagName == null)
                    {
                        isQuotedImplicit = true;
                    }

                    state = states.Pop();
                    ParsingEvent evt = new Events.Scalar(anchorName, tagName, scalar.Value, scalar.Style, isPlainImplicit, isQuotedImplicit, start, scalar.End);

                    Skip();
                    return evt;
                }

                var flowSequenceStart = GetCurrentToken() as FlowSequenceStart;
                if (flowSequenceStart != null)
                {
                    state = ParserState.FlowSequenceFirstEntry;
                    return new Events.SequenceStart(anchorName, tagName, isImplicit, SequenceStyle.Flow, start, flowSequenceStart.End);
                }

                var flowMappingStart = GetCurrentToken() as FlowMappingStart;
                if (flowMappingStart != null)
                {
                    state = ParserState.FlowMappingFirstKey;
                    return new Events.MappingStart(anchorName, tagName, isImplicit, MappingStyle.Flow, start, flowMappingStart.End);
                }

                if (isBlock)
                {
                    var blockSequenceStart = GetCurrentToken() as BlockSequenceStart;
                    if (blockSequenceStart != null)
                    {
                        state = ParserState.BlockSequenceFirstEntry;
                        return new Events.SequenceStart(anchorName, tagName, isImplicit, SequenceStyle.Block, start, blockSequenceStart.End);
                    }

                    var blockMappingStart = GetCurrentToken() as BlockMappingStart;
                    if (blockMappingStart != null)
                    {
                        state = ParserState.BlockMappingFirstKey;
                        return new Events.MappingStart(anchorName, tagName, isImplicit, MappingStyle.Block, start, GetCurrentToken().End);
                    }
                }

                if (anchorName != null || tag != null)
                {
                    state = states.Pop();
                    return new Events.Scalar(anchorName, tagName, string.Empty, ScalarStyle.Plain, isImplicit, false, start, GetCurrentToken().End);
                }

                var current = GetCurrentToken();
                throw new SemanticErrorException(current.Start, current.End, "While parsing a node, did not find expected node content.");
            }
        }

        /// <summary>
        /// Parse the productions:
        /// implicit_document    ::= block_node DOCUMENT-END*
        ///                                     *************
        /// explicit_document    ::= DIRECTIVE* DOCUMENT-START block_node? DOCUMENT-END*
        ///                                                                *************
        /// </summary>

        private ParsingEvent ParseDocumentEnd()
        {
            bool isImplicit = true;
            Mark start = GetCurrentToken().Start;
            Mark end = start;

            if (GetCurrentToken() is DocumentEnd)
            {
                end = GetCurrentToken().End;
                Skip();
                isImplicit = false;
            }
            else if (!(currentToken is StreamEnd || currentToken is DocumentStart))
            {
                throw new SemanticErrorException(start, end, "Did not find expected <document end>.");
            }

            state = ParserState.DocumentStart;
            return new Events.DocumentEnd(isImplicit, start, end);
        }

        /// <summary>
        /// Parse the productions:
        /// block_sequence ::= BLOCK-SEQUENCE-START (BLOCK-ENTRY block_node?)* BLOCK-END
        ///                    ********************  *********** *             *********
        /// </summary>

        private ParsingEvent ParseBlockSequenceEntry(bool isFirst)
        {
            if (isFirst)
            {
                GetCurrentToken();
                Skip();
            }

            if (GetCurrentToken() is BlockEntry)
            {
                Mark mark = GetCurrentToken().End;

                Skip();
                if (!(GetCurrentToken() is BlockEntry || GetCurrentToken() is BlockEnd))
                {
                    states.Push(ParserState.BlockSequenceEntry);
                    return ParseNode(true, false);
                }
                else
                {
                    state = ParserState.BlockSequenceEntry;
                    return ProcessEmptyScalar(mark);
                }
            }
            else if (GetCurrentToken() is BlockEnd)
            {
                state = states.Pop();
                ParsingEvent evt = new Events.SequenceEnd(GetCurrentToken().Start, GetCurrentToken().End);
                Skip();
                return evt;
            }
            else
            {
                var current = GetCurrentToken();
                throw new SemanticErrorException(current.Start, current.End, "While parsing a block collection, did not find expected '-' indicator.");
            }
        }

        /// <summary>
        /// Parse the productions:
        /// indentless_sequence  ::= (BLOCK-ENTRY block_node?)+
        ///                           *********** *
        /// </summary>
        private ParsingEvent ParseIndentlessSequenceEntry()
        {
            if (GetCurrentToken() is BlockEntry)
            {
                Mark mark = GetCurrentToken().End;
                Skip();

                if (!(GetCurrentToken() is BlockEntry || GetCurrentToken() is Key || GetCurrentToken() is Value || GetCurrentToken() is BlockEnd))
                {
                    states.Push(ParserState.IndentlessSequenceEntry);
                    return ParseNode(true, false);
                }
                else
                {
                    state = ParserState.IndentlessSequenceEntry;
                    return ProcessEmptyScalar(mark);
                }
            }
            else
            {
                state = states.Pop();
                return new Events.SequenceEnd(GetCurrentToken().Start, GetCurrentToken().End);
            }
        }

        /// <summary>
        /// Parse the productions:
        /// block_mapping        ::= BLOCK-MAPPING_START
        ///                          *******************
        ///                          ((KEY block_node_or_indentless_sequence?)?
        ///                            *** *
        ///                          (VALUE block_node_or_indentless_sequence?)?)*
        ///
        ///                          BLOCK-END
        ///                          *********
        /// </summary>
        private ParsingEvent ParseBlockMappingKey(bool isFirst)
        {
            if (isFirst)
            {
                GetCurrentToken();
                Skip();
            }

            if (GetCurrentToken() is Key)
            {
                Mark mark = GetCurrentToken().End;
                Skip();
                if (!(GetCurrentToken() is Key || GetCurrentToken() is Value || GetCurrentToken() is BlockEnd))
                {
                    states.Push(ParserState.BlockMappingValue);
                    return ParseNode(true, true);
                }
                else
                {
                    state = ParserState.BlockMappingValue;
                    return ProcessEmptyScalar(mark);
                }
            }

            else if (GetCurrentToken() is Value value)
            {
                Skip();
                return ProcessEmptyScalar(value.End);
            }

            else if (GetCurrentToken() is AnchorAlias anchorAlias)
            {
                Skip();
                return new Events.AnchorAlias(anchorAlias.Value, anchorAlias.Start, anchorAlias.End);
            }

            else if (GetCurrentToken() is BlockEnd)
            {
                state = states.Pop();
                ParsingEvent evt = new Events.MappingEnd(GetCurrentToken().Start, GetCurrentToken().End);
                Skip();
                return evt;
            }

            else
            {
                var current = GetCurrentToken();
                throw new SemanticErrorException(current.Start, current.End, "While parsing a block mapping, did not find expected key.");
            }
        }

        /// <summary>
        /// Parse the productions:
        /// block_mapping        ::= BLOCK-MAPPING_START
        ///
        ///                          ((KEY block_node_or_indentless_sequence?)?
        ///
        ///                          (VALUE block_node_or_indentless_sequence?)?)*
        ///                           ***** *
        ///                          BLOCK-END
        ///
        /// </summary>
        private ParsingEvent ParseBlockMappingValue()
        {
            if (GetCurrentToken() is Value)
            {
                Mark mark = GetCurrentToken().End;
                Skip();

                if (!(GetCurrentToken() is Key || GetCurrentToken() is Value || GetCurrentToken() is BlockEnd))
                {
                    states.Push(ParserState.BlockMappingKey);
                    return ParseNode(true, true);
                }
                else
                {
                    state = ParserState.BlockMappingKey;
                    return ProcessEmptyScalar(mark);
                }
            }

            else
            {
                state = ParserState.BlockMappingKey;
                return ProcessEmptyScalar(GetCurrentToken().Start);
            }
        }

        /// <summary>
        /// Parse the productions:
        /// flow_sequence        ::= FLOW-SEQUENCE-START
        ///                          *******************
        ///                          (flow_sequence_entry FLOW-ENTRY)*
        ///                           *                   **********
        ///                          flow_sequence_entry?
        ///                          *
        ///                          FLOW-SEQUENCE-END
        ///                          *****************
        /// flow_sequence_entry  ::= flow_node | KEY flow_node? (VALUE flow_node?)?
        ///                          *
        /// </summary>
        private ParsingEvent ParseFlowSequenceEntry(bool isFirst)
        {
            if (isFirst)
            {
                GetCurrentToken();
                Skip();
            }

            ParsingEvent evt;
            if (!(GetCurrentToken() is FlowSequenceEnd))
            {
                if (!isFirst)
                {
                    if (GetCurrentToken() is FlowEntry)
                    {
                        Skip();
                    }
                    else
                    {
                        var current = GetCurrentToken();
                        throw new SemanticErrorException(current.Start, current.End, "While parsing a flow sequence, did not find expected ',' or ']'.");
                    }
                }

                if (GetCurrentToken() is Key)
                {
                    state = ParserState.FlowSequenceEntryMappingKey;
                    evt = new Events.MappingStart(null, null, true, MappingStyle.Flow);
                    Skip();
                    return evt;
                }
                else if (!(GetCurrentToken() is FlowSequenceEnd))
                {
                    states.Push(ParserState.FlowSequenceEntry);
                    return ParseNode(false, false);
                }
            }

            state = states.Pop();
            evt = new Events.SequenceEnd(GetCurrentToken().Start, GetCurrentToken().End);
            Skip();
            return evt;
        }

        /// <summary>
        /// Parse the productions:
        /// flow_sequence_entry  ::= flow_node | KEY flow_node? (VALUE flow_node?)?
        ///                                      *** *
        /// </summary>
        private ParsingEvent ParseFlowSequenceEntryMappingKey()
        {
            if (!(GetCurrentToken() is Value || GetCurrentToken() is FlowEntry || GetCurrentToken() is FlowSequenceEnd))
            {
                states.Push(ParserState.FlowSequenceEntryMappingValue);
                return ParseNode(false, false);
            }
            else
            {
                Mark mark = GetCurrentToken().End;
                Skip();
                state = ParserState.FlowSequenceEntryMappingValue;
                return ProcessEmptyScalar(mark);
            }
        }

        /// <summary>
        /// Parse the productions:
        /// flow_sequence_entry  ::= flow_node | KEY flow_node? (VALUE flow_node?)?
        ///                                                      ***** *
        /// </summary>
        private ParsingEvent ParseFlowSequenceEntryMappingValue()
        {
            if (GetCurrentToken() is Value)
            {
                Skip();
                if (!(GetCurrentToken() is FlowEntry || GetCurrentToken() is FlowSequenceEnd))
                {
                    states.Push(ParserState.FlowSequenceEntryMappingEnd);
                    return ParseNode(false, false);
                }
            }
            state = ParserState.FlowSequenceEntryMappingEnd;
            return ProcessEmptyScalar(GetCurrentToken().Start);
        }

        /// <summary>
        /// Parse the productions:
        /// flow_sequence_entry  ::= flow_node | KEY flow_node? (VALUE flow_node?)?
        ///                                                                      *
        /// </summary>
        private ParsingEvent ParseFlowSequenceEntryMappingEnd()
        {
            state = ParserState.FlowSequenceEntry;
            return new Events.MappingEnd(GetCurrentToken().Start, GetCurrentToken().End);
        }

        /// <summary>
        /// Parse the productions:
        /// flow_mapping         ::= FLOW-MAPPING-START
        ///                          ******************
        ///                          (flow_mapping_entry FLOW-ENTRY)*
        ///                           *                  **********
        ///                          flow_mapping_entry?
        ///                          ******************
        ///                          FLOW-MAPPING-END
        ///                          ****************
        /// flow_mapping_entry   ::= flow_node | KEY flow_node? (VALUE flow_node?)?
        ///                          *           *** *
        /// </summary>
        private ParsingEvent ParseFlowMappingKey(bool isFirst)
        {
            if (isFirst)
            {
                GetCurrentToken();
                Skip();
            }

            if (!(GetCurrentToken() is FlowMappingEnd))
            {
                if (!isFirst)
                {
                    if (GetCurrentToken() is FlowEntry)
                    {
                        Skip();
                    }
                    else
                    {
                        var current = GetCurrentToken();
                        throw new SemanticErrorException(current.Start, current.End, "While parsing a flow mapping,  did not find expected ',' or '}'.");
                    }
                }

                if (GetCurrentToken() is Key)
                {
                    Skip();

                    if (!(GetCurrentToken() is Value || GetCurrentToken() is FlowEntry || GetCurrentToken() is FlowMappingEnd))
                    {
                        states.Push(ParserState.FlowMappingValue);
                        return ParseNode(false, false);
                    }
                    else
                    {
                        state = ParserState.FlowMappingValue;
                        return ProcessEmptyScalar(GetCurrentToken().Start);
                    }
                }
                else if (GetCurrentToken() is Scalar)
                {
                    states.Push(ParserState.FlowMappingValue);
                    return ParseNode(false, false);
                }
                else if (!(GetCurrentToken() is FlowMappingEnd))
                {
                    states.Push(ParserState.FlowMappingEmptyValue);
                    return ParseNode(false, false);
                }
            }

            state = states.Pop();
            ParsingEvent evt = new Events.MappingEnd(GetCurrentToken().Start, GetCurrentToken().End);
            Skip();
            return evt;
        }

        /// <summary>
        /// Parse the productions:
        /// flow_mapping_entry   ::= flow_node | KEY flow_node? (VALUE flow_node?)?
        ///                                   *                  ***** *
        /// </summary>
        private ParsingEvent ParseFlowMappingValue(bool isEmpty)
        {
            if (isEmpty)
            {
                state = ParserState.FlowMappingKey;
                return ProcessEmptyScalar(GetCurrentToken().Start);
            }

            if (GetCurrentToken() is Value)
            {
                Skip();
                if (!(GetCurrentToken() is FlowEntry || GetCurrentToken() is FlowMappingEnd))
                {
                    states.Push(ParserState.FlowMappingKey);
                    return ParseNode(false, false);
                }
            }

            state = ParserState.FlowMappingKey;
            return ProcessEmptyScalar(GetCurrentToken().Start);
        }

        private class EventQueue
        {
            // This class is specialized for our specific use case where there are exactly two priority levels.
            // If more levels are required, a more generic implementation should be used instead.
            private readonly Queue<ParsingEvent> highPriorityEvents = new Queue<ParsingEvent>();
            private readonly Queue<ParsingEvent> normalPriorityEvents = new Queue<ParsingEvent>();

            public void Enqueue(ParsingEvent @event)
            {
                switch (@event.Type)
                {
                    case Events.EventType.StreamStart:
                    case Events.EventType.DocumentStart:
                        highPriorityEvents.Enqueue(@event);
                        break;

                    default:
                        normalPriorityEvents.Enqueue(@event);
                        break;
                }
            }

            public ParsingEvent Dequeue()
            {
                return highPriorityEvents.Count > 0
                    ? highPriorityEvents.Dequeue()
                    : normalPriorityEvents.Dequeue();
            }

            public int Count
            {
                get
                {
                    return highPriorityEvents.Count + normalPriorityEvents.Count;
                }
            }
        }
    }
}
