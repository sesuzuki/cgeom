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
    CGEOM_QUANT_API void cgeomLocalBasis(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outCount, double **outX1Coords, double **outX2Coords, double **outX3Coords);

    CGEOM_QUANT_API int cgeomPerFaceAsymptoticDirections(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1Count, size_t *outX2Count, size_t *outVanishingCount, double **outX1Coords, double **outX2Coords, int **outVanishing, const char **errorMessage);

    CGEOM_QUANT_API int cgeomPerVertexAsymptoticDirections(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1Count, size_t *outX2Count, size_t *outVanishingCount, double **outX1, double **outX2, int **outVanishing, const char **errorMessage);

    CGEOM_QUANT_API void cgeomPerVertexPrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outK1Count, size_t *outK2Count, double **outX1Coords, double **outX2Coords, double **outK1, double **outK2);
    
    CGEOM_QUANT_API void cgeomPerFacePrincipalCurvatures(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t *outX1CoordsCount, size_t *outX2CoordsCount, size_t *outK1Count, size_t *outK2Count, double **outX1Coords, double **outX2Coords, double **outK1, double **outK2);
    
    CGEOM_QUANT_API void cgeomNormalsPerVertex(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outNorm);
    
    CGEOM_QUANT_API void cgeomNormalsPerFace(const int numVertices, const int numFaces, double *inCoords, int *inFaces,  double **outNorm);
    
    CGEOM_QUANT_API void cgeomNormalsPerCorner(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double angle,  double **outNorm);

    CGEOM_QUANT_API void cgeomGaussianCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outK);

    CGEOM_QUANT_API void cgeomMeanCurvature(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double **outH);

    CGEOM_QUANT_API void cgeomLaplacianSmoothingForOpenMesh(const int numVertices, const int numFaces, const int numBoundaries, double *inCoords, int *inFaces, int *inBoundaries, int *inInteriors, int numIterations, size_t * outCoordsCount, double **outCoords);

    CGEOM_QUANT_API void cgeomLaplacianSmoothingForCloseMesh(const int numVertices, const int numFaces, double *inCoords, int *inFaces, double smoothing, int numIterations, size_t * outCoordsCount, double **outCoords);

    CGEOM_QUANT_API void cgeomBarycenters(const int numVertices, const int numFaces, double *inCoords, int *inFaces, size_t * outCoordsCount, double **outCoords);

    CGEOM_QUANT_API int cgeomExactDiscreteGeodesicDistances(const int numVertices, const int numFaces, const int numSourceVertices, const int numSourceFaces, const int numTargetVertices, const int numTargetFaces, double *inCoords, int *inFaces, int *inSourceVertices, int *inSourceFaces, int *inTargetVertices, int *inTargetFaces, size_t *outDistancesCount, double **outDistances, const char **errorMessage);

    CGEOM_QUANT_API int cgeomHeatGeodesics(const int numVertices, const int numFaces, double *inCoords, int *inFaces, const double t, const int inSourceVertex, const bool useIntrinsicDelaunay, size_t *outCount, double **outDistances, const char **errorMessage);

    CGEOM_QUANT_API int cgeomGeodesicPaths(const int numVertices, const int numFaces, const int numSourceVertices, const int numTargetVertices, double *inCoords, int *inFaces, int *inSourceVertices, int *inTargetVertices, size_t *outCount, double **outCoords, int **outCoordsSize, const char **errorMessage);
}