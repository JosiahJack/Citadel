#Blender 2.7 Addon to take the active face and perform loop cuts
#based on differences between pixels in its diffuse texture.
#Cuts are made at the line made between pixels.  E.g. a 2x2 texture
#will be split down the center of the face provided the pixels at
#1,0 & 1,1 are different enough from 0,0 and 0,1.

#Pseudo-code:
#UI
#3D View->Sub-Panel on tool bar
#->->Button["Edge Detecct To Edges for Active Face"]
#->->Dropdown Menu["Mode",Distance Based,Luminosity Based]
#->->Number Input box["Threshhold",1<n<256]

#Code flow
#get active object
#get faces from mesh data
#get active face
#get texture for face
#get UV coords for face
#set scaleVariable to ratio of pixels to texels from UV coords based on
# difference from width and height of face vs width and height of UV coords
# e.g. UV fills entire texture = 1:1, e.g. UV fills 10% of texture = 10:1
#set rotationVariable of texture from UV
#for each pixel in texture starting with upper left pixel_X + 1 with UV coords for active face
#	Check if pixel is different enough() from neighboring pixel x-1
#		edge loop cut across entire face at rotationVariable at position between pixels from "left" edge of UV
#for each pixel in texture starting with upper left pixel_y + 1 with UV coords for active face
#	check if pixel is different enough() from neighboring pixel y-1
#		edge loop cut across entire face at rotationVariable at position between pixels from "top" edge of UV

#Check if pixel is different enough(2d texel, 2d neighbor, threshhold)
#->Mode Luminosity Based
#	Sum r,g, and b values of texel
#   Sum r,g, and b values of neighbor
#	return true if texel sum - neighbor sum is > threshhold
#
#->Mode Distance Based
#	get distance from texel to neighbor in color space (rgb tuple to rgb tuple)
#	return true if distance > threshhold
bl_info = {
    "name": "Edge Detect 2 Edges",
    "author": "Josiiah Jack",
    "version": (1, 0, 0),
    "blender": (2, 78, 0),
    "location": "View3D > Specials > EdgeDetect2Edges",
    "description": "Cuts active face based on texture edge detection",
    "warning": "",
    "category": "Mesh",
}

import bpy
import bmesh
import math
from bpy.props import IntProperty

def edgedetect2edges_main(context, thresh):
	ob = bpy.context.object
	if ob.mode == 'EDIT':
		bpy.ops.object.editmode_toggle()
		bpy.ops.object.editmode_toggle()
	
	p = ob.data.polygons
	mat = ob.data.materials[p[p.active].material_index]	
	tex = mat.active_texture
	#img = bpy.data.textures.get()
	texWidth = tex.size[0]
	texHeight = tex.size[1]
	for x in range(2,texWidth):
		for y in range(2,texHeight):
			i = x*y
			t = tex.pixels[i]
			i_neighbor = (x-1)*y
			n = tex.pixels[i_neighbor]
			if isDifferenceEnough(t,n,thresh):
				make_cut_x(x,y)
				
			i_neighbor = x*(y-1)
			n = tex.pixels[i_neighbor]
			if isDifferenceEnough(t,n,thresh):
				make_cut_y(x,y)

def isDifferenceEnough(texel, neighbor, thresh):
	diff = math.sqrt((texel_x - neighbor_x)^2 + (texel_y - neighbor_y)^2 + (texel_z - neighbor_z)^2)
	return diff > thresh
	
def make_cut_x(x,y):
	#Slice it x-wise relative to UV coords rotationVariable
	bmesh.utils.face_split(f,pointA,pointB,coords=(),False,None)
	
def make_cut_y(x,y)
	#Slice it y-wise relative to UV coords rotationVariable
	
class EdgeDetect2Edges(bpy.types.Operator):
    bl_idname = 'mesh.edgedetecte2edges'
    bl_label = 'Edge Detect 2 Edges'
    bl_options = {'REGISTER', 'UNDO'}

    threshhold = IntProperty(name="Threshhold",
                default=100, min=1, max=256, soft_min=1, soft_max=256)

    @classmethod
    def poll(cls, context):
        obj = context.active_object
        return (obj and obj.type == 'MESH')

    def execute(self, context):
        edgedetect2edges_main(context, threshhold)
        return {'FINISHED'}


def menu_func(self, context):
    self.layout.operator(EdgeDetect2Edges.bl_idname, text="EdgeDetect 2 Edges")


def register():
    bpy.utils.register_module(__name__)

    bpy.types.VIEW3D_MT_edit_mesh_specials.append(menu_func)
    bpy.types.VIEW3D_MT_edit_mesh_vertices.append(menu_func)

def unregister():
    bpy.utils.unregister_module(__name__)

    bpy.types.VIEW3D_MT_edit_mesh_specials.remove(menu_func)
    bpy.types.VIEW3D_MT_edit_mesh_vertices.remove(menu_func)

if __name__ == "__main__":
    register()
