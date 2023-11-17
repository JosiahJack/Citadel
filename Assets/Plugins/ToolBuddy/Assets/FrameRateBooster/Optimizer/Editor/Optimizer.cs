#if UNITY_2021_1_OR_NEWER
#define USE_IPOSTBUILDPLAYERSCRIPTDLLS
#elif UNITY_2019_3_OR_NEWER
#define USE_IIL2CPPPROCESSOR
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEditor.Callbacks;
#if USE_IPOSTBUILDPLAYERSCRIPTDLLS  || USE_IIL2CPPPROCESSOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
#if USE_IIL2CPPPROCESSOR
using UnityEditor.Il2Cpp;
#endif
using Debug = UnityEngine.Debug;

namespace ToolBuddy.FrameRateBooster.Optimizer
{

    public class Optimizer
#if USE_IPOSTBUILDPLAYERSCRIPTDLLS
        : IPostBuildPlayerScriptDLLs
#elif USE_IIL2CPPPROCESSOR
        : IIl2CppProcessor
#endif
    {
        #region Unity callbacks
#if USE_IPOSTBUILDPLAYERSCRIPTDLLS
        public int callbackOrder { get; }

        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            if (PlayerSettings.GetScriptingBackend(report.summary.platformGroup) == ScriptingImplementation.IL2CPP)
            {
                string pathToBuiltProject = report.files.FirstOrDefault(r => r.path.Contains("StagingArea")).path;

                if (string.IsNullOrEmpty(pathToBuiltProject) == false)
                {
                    //Enabling the code line bellow will run the optimization in this case, but builds fail at linking step, and I don't know why, so I am ignoring the optimization.
                    //OptimizeBuild(report.summary.platform, Path.GetDirectoryName(Path.GetFullPath(pathToBuiltProject)));
                    Debug.LogWarning("[Frame Rate Booster] FRB can modify IL2CPP builds only on Unity versions between 2019.3 and 2020.3 inclusive.");
                }
                else
                {
                    Debug.LogWarning("[Frame Rate Booster] Could not find path for StagingArea");
                }

                Debug.LogWarning("[Frame Rate Booster] Using FRB on IL2CPP can make builds slower. More details at this link: https://forum.curvyeditor.com/thread-861.html");
            }
        }

#elif USE_IIL2CPPPROCESSOR
        public int callbackOrder { get; }
        
        public void OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
        {
            Debug.LogWarning("[Frame Rate Booster] Using FRB on IL2CPP can make builds slower. More details at this link: https://forum.curvyeditor.com/thread-861.html");

            OptimizeBuild(report.summary.platform, Path.GetDirectoryName(Path.GetFullPath(data.inputDirectory)));
        }
#endif

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (PlayerSettings.GetScriptingBackend(BuildPipeline.GetBuildTargetGroup(target)) == ScriptingImplementation.IL2CPP)
            {
#if USE_IIL2CPPPROCESSOR == false && USE_IPOSTBUILDPLAYERSCRIPTDLLS == false
                Debug.LogWarning("[Frame Rate Booster] FRB can modify IL2CPP builds only on Unity versions between 2019.3 and 2020.3 inclusive.");
                Debug.LogWarning("[Frame Rate Booster] Using FRB on IL2CPP can make builds slower. More details at this link: https://forum.curvyeditor.com/thread-861.html");
#endif
                return;
            }
            OptimizeBuild(target, Path.GetDirectoryName(Path.GetFullPath(pathToBuiltProject)));
        }
        #endregion

        private static void OptimizeBuild(BuildTarget target, string buildDirectoryPath)
        {
            if (target == BuildTarget.Android)
            {
                Debug.LogWarning("[Frame Rate Booster] Automatic optimization of Android builds is not supported");
                Debug.Log(
                    "[Frame Rate Booster] You can still optimize it manually, by unpacking the apk file, run the Optimize method on the content of the unpacked apk (specifically the assets\\bin\\Data folder) and then repack the apk. I personally have 0 experience making android apk files, so I can't help much more with this subject. If you find a way to automate this, please let me know.");
                return;
            }

#if UNITY_2017_3_OR_NEWER
            const bool optimizationInOwnAssembly = true;
#else
            const bool optimizationInOwnAssembly = false;
#endif

#if UNITY_2017_2_OR_NEWER
            const string targetAssemblyName = "UnityEngine.CoreModule.dll";
#else
            const string targetAssemblyName = "UnityEngine.dll";
#endif
            Debug.Log("[Frame Rate Booster] Started post build optimization");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string[] allAssembliesPaths = Directory.GetFiles(buildDirectoryPath, "*.dll", SearchOption.AllDirectories);

            string optimizationsAssemblyPath;
            {
                const string optimizationsAssemblyName = optimizationInOwnAssembly
                    ? "FrameRateBooster.Optimizations.dll"
                    : "Assembly-CSharp-firstpass.dll";

                if (GetUniqueTargetAssembly(optimizationsAssemblyName, allAssembliesPaths, buildDirectoryPath,
                    "the assembly containing the optimizations", out optimizationsAssemblyPath) == false)
                    return;
            }

            string targetAssemblyPath;
            {
                if (GetUniqueTargetAssembly(targetAssemblyName, allAssembliesPaths, buildDirectoryPath, "the assembly to optimize",
                    out targetAssemblyPath) == false)
                    return;
            }

            int optimizedMethodsCount = Optimize(optimizationsAssemblyPath, targetAssemblyPath, optimizationInOwnAssembly, !optimizationInOwnAssembly);

            stopWatch.Stop();

            Debug.Log("[Frame Rate Booster] Finished post build optimization. Operation took " + stopWatch.ElapsedMilliseconds +
                      " milliseconds and optimized " + optimizedMethodsCount + " methods and properties");
        }

        private static bool GetUniqueTargetAssembly(string targetAssemblyName, IEnumerable<string> allAssembliesPaths, string buildDirectory, string assemblyDescription, out string targetAssemblyPath)
        {
            List<string> assembliesToOptimize = allAssembliesPaths.Where(s => s.Contains(targetAssemblyName)).ToList();

            switch (assembliesToOptimize.Count())
            {
                case 1:
                    targetAssemblyPath = assembliesToOptimize.First();
                    break;
                case 0:
                    targetAssemblyPath = null;
                    Debug.LogError(String.Format("[Frame Rate Booster] Couldn't locate recursively {2} {0} in the build folder {1}", targetAssemblyName, buildDirectory, assemblyDescription));
                    return false;
                default:
                    targetAssemblyPath = null;
                    Debug.LogError(String.Format("[Frame Rate Booster] Located recursively multiple occurrences of {2} {0} in the build folder {1}", targetAssemblyName, buildDirectory, assemblyDescription));
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Replaces non optimized Unity operators (in target assembly) with optimized ones (from optimizations assembly)
        /// </summary>
        /// <param name="optimizationsAssemblyPath">The path to the assembly containing the optimized version of Unity's opertors</param>
        /// <param name="targetAssemblyPath">The path of the assembly to apply the optimizations on</param>
        /// <param name="deleteOptimizationsAssembly">Should the optimizations assembly be deleted after the optimization process is finished?</param>
        /// <param name="trimOptimizationsAssembly">Should the optimized operators be removed from the optimizations module after the  optimization process is finished?</param>
        /// <returns>The number of optimized methods</returns>
        static public int Optimize(string optimizationsAssemblyPath, string targetAssemblyPath, bool deleteOptimizationsAssembly, bool trimOptimizationsAssembly)
        {
            const string optimizedNameSpace = "ToolBuddy.FrameRateBooster.Optimizations";
            const string originalNameSpace = "UnityEngine";

            int optimizedMethodsCount = 0;

            using (ModuleDefinition optimizedModuleDefinition = ModuleDefinition.ReadModule(optimizationsAssemblyPath, new ReaderParameters() { ReadWrite = trimOptimizationsAssembly }))
            {
                using (ModuleDefinition originalModule = ModuleDefinition.ReadModule(targetAssemblyPath, new ReaderParameters() { ReadWrite = true }))
                {
                    foreach (TypeDefinition optimizedType in optimizedModuleDefinition.Types)
                    {
                        if (optimizedType.Namespace == optimizedNameSpace)
                        {
                            TypeDefinition originalType = GetOriginalTypeIfAny(originalModule, originalNameSpace, optimizedType);

                            if (originalType != null)
                            {
                                foreach (MethodDefinition optimizedMethod in optimizedType.Methods)
                                {
                                    MethodDefinition method = originalType.Methods.SingleOrDefault(m => m.Name == optimizedMethod.Name && m.ReturnType.Name == optimizedMethod.ReturnType.Name && m.Parameters.Count == optimizedMethod.Parameters.Count && m.Parameters.Select(p => p.ParameterType.Name).SequenceEqual(optimizedMethod.Parameters.Select(p => p.ParameterType.Name)));


                                    if (method == null)
                                    {
                                        Debug.Log(String.Format("[Frame Rate Booster] Couldn't find in assembly {2} any method to optimize that matches {0}.{1}. This optimization will be skipped.", optimizedMethod.DeclaringType, optimizedMethod.Name, originalModule.Name));
                                        continue;
                                    }

                                    method.Body.Variables.Clear();
                                    foreach (VariableDefinition variable in optimizedMethod.Body.Variables)
                                    {
                                        if (variable.VariableType.Namespace == optimizedNameSpace)
                                            variable.VariableType = GetOriginalType(originalModule, originalNameSpace, variable.VariableType);

                                        method.Body.Variables.Add(variable);
                                    }

                                    method.Body.MaxStackSize = optimizedMethod.Body.MaxStackSize;

                                    method.Body.Instructions.Clear();
                                    foreach (Instruction instruction in optimizedMethod.Body.Instructions)
                                    {
                                        Instruction newInstruction;

                                        FieldReference fieldReference = (instruction.Operand as FieldReference);
                                        MethodReference methodReference = (instruction.Operand as MethodReference);
                                        if (fieldReference != null && fieldReference.DeclaringType.Namespace == optimizedNameSpace)
                                            newInstruction = Instruction.Create(instruction.OpCode, new FieldReference(fieldReference.Name, fieldReference.FieldType, GetOriginalType(originalModule, originalNameSpace, fieldReference.DeclaringType)));
                                        else if (methodReference != null)
                                        {
                                            methodReference.DeclaringType = originalModule.ImportReference(methodReference.DeclaringType.Resolve());
                                            newInstruction = instruction;
                                        }
                                        else
                                            newInstruction = instruction;
                                        method.Body.GetILProcessor().Append(newInstruction);
                                    }
                                    optimizedMethodsCount++;
                                }
                            }
                        }
                    }

                    if (optimizedMethodsCount == 0)
                        Debug.LogError("[Frame Rate Booster] Couldn't find any method to optimize. This is not supposed to happen. Please report that to the asset creator.");

                    originalModule.Write();
                }

                if (deleteOptimizationsAssembly)
                {
                    optimizedModuleDefinition.Dispose();
                    File.Delete(optimizationsAssemblyPath);
                }
                else if (trimOptimizationsAssembly)
                {
                    List<int> indicesToRemove = new List<int>();
                    for (int i = 0; i < optimizedModuleDefinition.Types.Count; i++)
                    {
                        TypeDefinition optimizedType = optimizedModuleDefinition.Types[i];
                        if (optimizedType.Namespace == optimizedNameSpace)
                            indicesToRemove.Add(i);
                    }

                    indicesToRemove.Sort();
                    while (indicesToRemove.Any())
                    {
                        optimizedModuleDefinition.Types.RemoveAt(indicesToRemove.Last());
                        indicesToRemove.Remove(indicesToRemove.Last());

                    }
                    //TODO assure toi que la taille de assemblyCsharp ne grandit pas
                    optimizedModuleDefinition.Write();
                }
            }

            return optimizedMethodsCount;
        }

        static private TypeReference GetOriginalType(ModuleDefinition originalModule, string originalNameSpace, TypeReference optimizedType)
        {
            return originalModule.Types.Single(t => t.Name == optimizedType.Name && t.Namespace == originalNameSpace);
        }

        static private TypeDefinition GetOriginalTypeIfAny(ModuleDefinition originalModule, string originalNameSpace, TypeDefinition optimizedType)
        {
            return originalModule.Types.SingleOrDefault(t => t.Name == optimizedType.Name && t.Namespace == originalNameSpace);
        }

    }
}