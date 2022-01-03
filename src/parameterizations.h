#define CGEOM_PARAM_API_VERSION "cgeom"

#if defined(CGEOM_PARAM_DLL)
#if defined(CGEOM_PARAM_DLL_EXPORT)
#define CGEOM_PARAM_API __declspec(dllexport)
#else
#define CGEOM_PARAM_API __declspec(dllimport)
#endif
#else
#define CGEOM_PARAM_API __attribute__((visibility("default")))
#endif

namespace CGeom
{
    CGEOM_PARAM_API void iglSeamlessIntegerGridParameterization(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outVertexCount, size_t *outFaceCount, double **outVertexCoords, int **outFaceIndexes); 
}