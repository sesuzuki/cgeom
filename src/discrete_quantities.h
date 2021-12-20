#define CGEOM_QUANT_API_VERSION "cgeom discrete quantities 0.1.0"

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
    ///////////////////////////////////////////////
    // Discrete Geometric Quantities And Operators
    ///////////////////////////////////////////////

    CGEOM_QUANT_API void iglPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outDir1, double **outDir2, double **outVal1, double **outVal2);
    
    CGEOM_QUANT_API void iglNormalsPerVertex(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outNorm);
    
    CGEOM_QUANT_API void iglNormalsPerFace(const int numVertices, const int numFaces, double *inCoords, int *inFaces,  double **outNorm);
    
    CGEOM_QUANT_API void iglNormalsPerCorner(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double angle,  double **outNorm);

    CGEOM_QUANT_API void iglGaussianCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outK);

    CGEOM_QUANT_API void iglMeanCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outH);

    CGEOM_QUANT_API void iglLaplacianSmoothing(const int numVertices, const int numFaces, double *inCoords, int *inFaces, int numIterations, double **outCoords);
}