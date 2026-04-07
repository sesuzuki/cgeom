#pragma once
#include <vector>
#include <string>

// Thin wrapper that isolates Instant Meshes headers (which define VectorXi,
// MatrixXf etc. in the global namespace) from the rest of the codebase.
//
// Runs Instant Meshes field-aligned remeshing entirely in memory.
// inCoords:  row-major doubles [x0,y0,z0, x1,y1,z1, ...], length 3*numVertices
// inFaces:   row-major ints   [f0v0,f0v1,f0v2, f1v0,...], length 3*numFaces
// rosy:      rotational symmetry (6 = triangle-aligned, 4 = quad-aligned)
// posy:      positional symmetry (3 = triangles, 4 = quads)
// scale:     target edge length
//
// On success: outVertices, outFaceIndices, outFaceDegrees are filled and
//             the returned string is empty.
// On failure: returned string contains the error message.
// scale > 0        : use edge length directly
// targetVertexCount > 0 : derive scale from surface area / vertex count (scale is ignored)
// If both are <= 0 : defaults to 1/16 of input vertex count (Instant Meshes default)
// scale > 0        : use edge length directly
// targetVertexCount > 0 : derive scale from surface area / vertex count (scale is ignored)
// If both are <= 0 : defaults to 1/16 of input vertex count (Instant Meshes default)
// outActualScale   : the scale value actually used (may be clamped from input)
std::string instantMeshesProcess(
    const double* inCoords, int numVertices,
    const int*    inFaces,  int numFaces,
    int rosy, int posy, float scale, int targetVertexCount,
    std::vector<float>& outVertices,    // 3*nV floats, row-major: [x,y,z, ...]
    std::vector<int>&   outFaceIndices, // flat list of 0-based face vertex indices
    std::vector<int>&   outFaceDegrees, // per-face vertex count
    float& outActualScale               // scale value used after clamping
);
