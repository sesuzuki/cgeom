using static CGeom.Utils.ColorMaps;
using Rhino.Geometry;
using System.Linq;
using System.Drawing;
using static CGeom.Tools.DiscreteQuantities;
using System;

namespace CGeom.Utils
{
	public static class MetricsVisualization
	{

        public static void BuildColoredMeshMetrics(Mesh mesh, CurvatureMetricTypes mType, ColorMapTypes cMap, int alpha, out Mesh cMesh, out double[] data)
        {
            switch (mType)
            {
                case CurvatureMetricTypes.Gaussian:
                    data = GaussianCurvature(mesh);
                    break;
                case CurvatureMetricTypes.Mean:
                    data = MeanCurvature(mesh);
                    break;
                default:
                    data = GaussianCurvature(mesh);
                    break;
            }

            // Apply a rank transformation to deal with skewed data sets
            var rankData = RankTransform(data);

            int numData = data.Count();

            cMesh = mesh.DuplicateMesh();
            cMesh.UnifyNormals();
            cMesh.Normals.ComputeNormals();

            double min = rankData.Min();
            double max = rankData.Max();
            double range = max - min;

            Color[] colormap = ColorMaps.GetColorMap(cMap, alpha == 0 ? 1 : alpha);

            for (int i = 0; i < numData; i++)
            {
                double normData = (rankData[i] - min) / range;

                int colorIndex = (int)(normData * (colormap.Length - 1));
                cMesh.VertexColors.Add(colormap[colorIndex]);
            }
        }

        private static double[] RankTransform(double[] data)
        {
            // Create an array to store the ranks
            double[] rankedCurvatures = new double[data.Length];

            // Get the sorted indices
            int[] sortedIndices = data.Select((value, index) => new { Value = value, Index = index })
                                                 .OrderBy(item => item.Value)
                                                 .Select(item => item.Index)
                                                 .ToArray();

            // Assign ranks based on sorted indices
            for (int i = 0; i < sortedIndices.Length; i++)
            {
                rankedCurvatures[sortedIndices[i]] = i + 1; // Adding 1 to start the ranks from 1
            }

            return rankedCurvatures;
        }
    }
}

