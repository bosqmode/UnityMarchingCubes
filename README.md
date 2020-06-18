# UnityMarchingCubes

My attempt of a Marching cubes algorithm in Unity based on the Acknowledgments below.

![gif](https://i.imgur.com/3FClpl2.gif)

### Requirements

Unity 2018.3+ (might work with older ones, just not tested with)
Tested with a Samsung S8 (Android 8.0.0)

## Getting Started

Open up the project and in the Assets/MarchingCubes-folder
one can find 2 example-scenes.
One using a random populator and one for AR.

## MarchingCubes.cs
This is the heart of this project.

### Resolution
This sets the resolution for the nodes/grid.
1 being one unity unit
2 being 0.5 an so on.

### Populate(Vector3) and Populate(Vector3[])
Populates (adds a point to the grid and recalculates the changes for that addition)
a world-space position or positions.

### Delete(Vector3) and Delete(Vector3[])
Deletes a point or points in a world-space coordinate and
recalculates the changes.

## MCRenderer
A renderer component for the MarchingCubes.cs

### Recalculate Bounds Normals Tangents
One can squeeze a bit more performance by disabling this option
if bounds, normals and tangents are not needed for the mesh generated.

## Populators (RandomPopulator.cs and ARFoundationPopulator.cs)
These are the example populators for the system.

One can make own populators easily by calling the Populate() and Delete() functions
of the MarchingCubes.cs in his/hers own script. (Take a look at the forementioned populator scripts).

## Acknowledgments

* Inspiration: http://paulbourke.net/geometry/polygonise/
* and http://www.cs.carleton.edu/cs_comps/0405/shape/marching_cubes.html

