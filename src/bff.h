#define CGEOM_BFF_API_VERSION "cgeom"

#if defined(CGEOM_BFF_DLL)
#if defined(CGEOM_BFFDLL_EXPORT)
#define CGEOM_BFF_API __declspec(dllexport)
#else
#define CGEOM_BFF_API __declspec(dllimport)
#endif
#else
#define CGEOM_BFF_API __attribute__((visibility("default")))
#endif

namespace CGeom
{
    CGEOM_BFF_API void cgeomOpenSurfaceWithoutHolesToDisk(const int numVertices, const int numFaces, const double *inCoords, const int *inFaces, int *outNumUV, double **outUV, int *outNumFaces, int **outFaces, const char **errorMessage);
}