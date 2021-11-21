#define CGEOM_API_VERSION "cgeom"

#if defined(CGEOM_DLL)
#if defined(CGEOM_DLL_EXPORT)
#define CGEOM_API __declspec(dllexport)
#else
#define CGEOM_API __declspec(dllimport)
#endif
#else
#define CGEOM_API __attribute__((visibility("default")))
#endif

namespace CGeom
{
    CGEOM_API void iglPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outDir1, double **outDir2, double **outVal1, double **outVal2);
}