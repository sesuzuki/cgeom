#define CGEOM_QUANT_API_VERSION "cgeom"

#if defined(CGEOM_QUANT_DLL)
#if defined(CGEOM_QUANT_DLL_EXPORT)
#define CGEOM_QUANT_API __declspec(dllexport)
#else
#define CGEOM_QUANT_API __declspec(dllimport)
#endif
#else
#define CGEOM_QUANT_API __attribute__((visibility("default")))
#endif

namespace CGeom
{
    CGEOM_QUANT_API void cgeomPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outDir1, double **outDir2, double **outVal1, double **outVal2);
    
    CGEOM_QUANT_API void cgeomNormalsPerVertex(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outNorm);
    
    CGEOM_QUANT_API void cgeomNormalsPerFace(const int numVertices, const int numFaces, double *inCoords, int *inFaces,  double **outNorm);
    
    CGEOM_QUANT_API void cgeomNormalsPerCorner(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double angle,  double **outNorm);

    CGEOM_QUANT_API void cgeomGaussianCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outK);

    CGEOM_QUANT_API void cgeomMeanCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outH);

    CGEOM_QUANT_API void cgeomLaplacianSmoothingForOpenMesh(const int numVertices, const int numFaces, const int numBoundaries, double *inCoords, int *inFaces, int *inBoundaries, int *inInteriors, int numIterations, double **outCoords);

    CGEOM_QUANT_API void cgeomLaplacianSmoothingForCloseMesh(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double smoothing, int numIterations, double **outCoords);
}