#define CGEOM_REMESH_API_VERSION "cgeom remesh 0.1.0"

#if defined(CGEOM_REMESH_DLL)
#if defined(CGEOM_REMESH_DLL_EXPORT)
#define CGEOM_REMESH_API __declspec(dllexport)
#else
#define CGEOM_REMESH_API __declspec(dllimport)
#endif
#else
#define CGEOM_REMESH_API __attribute__((visibility("default")))
#endif

namespace CGeom
{
    CGEOM_REMESH_API void iglQuadRemeshing(const int numVertices, const int numFaces, double *inCoords, int *inFaces); 
}