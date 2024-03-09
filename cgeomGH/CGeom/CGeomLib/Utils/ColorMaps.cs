using System.Drawing;

namespace CGeom.Utils
{
    public partial class ColorMaps
    {
        public enum ColorMapTypes { Plasma, Viridis, Blues, BuGn, YlGnBu, CoolWarm, Cool, Turbo }

        public static Color[] GetColorMap(ColorMapTypes colorMapType, int alpha)
        {
            Color[] colormap;
            switch (colorMapType)
            {
                case ColorMapTypes.Plasma:
                    colormap = ColorMaps.Plasma.GetColors(alpha);
                    break;
                case ColorMapTypes.Viridis:
                    colormap = ColorMaps.Viridis.GetColors(alpha);
                    break;
                case ColorMapTypes.Blues:
                    colormap = ColorMaps.Blues.GetColors(alpha);
                    break;
                case ColorMapTypes.BuGn:
                    colormap = ColorMaps.BuGn.GetColors(alpha);
                    break;
                case ColorMapTypes.YlGnBu:
                    colormap = ColorMaps.YlGnBu.GetColors(alpha);
                    break;
                case ColorMapTypes.CoolWarm:
                    colormap = ColorMaps.CoolWarm.GetColors(alpha);
                    break;
                case ColorMapTypes.Cool:
                    colormap = ColorMaps.Cool.GetColors(alpha);
                    break;
                case ColorMapTypes.Turbo:
                    colormap = ColorMaps.Turbo.GetColors(alpha);
                    break;
                default:
                    colormap = ColorMaps.Plasma.GetColors(alpha);
                    break;

            }

            return colormap;
        }

        #region Perceptually Uniform Sequential colormaps
        private static class Plasma
        {
            public static Color[] GetColors(int alpha = 255)
            {
                Color[] colorRange = {
                    Color.FromArgb(alpha, 12, 7, 134),
                    Color.FromArgb(alpha, 27, 6, 140),
                    Color.FromArgb(alpha, 37, 5, 145),
                    Color.FromArgb(alpha, 47, 4, 149),
                    Color.FromArgb(alpha, 56, 4, 153),
                    Color.FromArgb(alpha, 66, 3, 157),
                    Color.FromArgb(alpha, 74, 2, 160),
                    Color.FromArgb(alpha, 82, 1, 163),
                    Color.FromArgb(alpha, 90, 0, 165),
                    Color.FromArgb(alpha, 100, 0, 167),
                    Color.FromArgb(alpha, 108, 0, 168),
                    Color.FromArgb(alpha, 115, 0, 168),
                    Color.FromArgb(alpha, 123, 2, 168),
                    Color.FromArgb(alpha, 130, 4, 167),
                    Color.FromArgb(alpha, 139, 9, 164),
                    Color.FromArgb(alpha, 146, 15, 162),
                    Color.FromArgb(alpha, 153, 20, 159),
                    Color.FromArgb(alpha, 159, 26, 155),
                    Color.FromArgb(alpha, 167, 33, 151),
                    Color.FromArgb(alpha, 173, 38, 146),
                    Color.FromArgb(alpha, 178, 44, 142),
                    Color.FromArgb(alpha, 184, 50, 137),
                    Color.FromArgb(alpha, 189, 55, 132),
                    Color.FromArgb(alpha, 195, 62, 127),
                    Color.FromArgb(alpha, 200, 68, 122),
                    Color.FromArgb(alpha, 205, 73, 117),
                    Color.FromArgb(alpha, 209, 79, 113),
                    Color.FromArgb(alpha, 215, 86, 108),
                    Color.FromArgb(alpha, 219, 91, 103),
                    Color.FromArgb(alpha, 223, 97, 99),
                    Color.FromArgb(alpha, 227, 103, 95),
                    Color.FromArgb(alpha, 230, 109, 90),
                    Color.FromArgb(alpha, 234, 116, 85),
                    Color.FromArgb(alpha, 237, 123, 81),
                    Color.FromArgb(alpha, 240, 129, 77),
                    Color.FromArgb(alpha, 243, 135, 72),
                    Color.FromArgb(alpha, 246, 143, 67),
                    Color.FromArgb(alpha, 248, 150, 63),
                    Color.FromArgb(alpha, 250, 157, 58),
                    Color.FromArgb(alpha, 251, 164, 54),
                    Color.FromArgb(alpha, 252, 172, 50),
                    Color.FromArgb(alpha, 253, 181, 45),
                    Color.FromArgb(alpha, 253, 188, 42),
                    Color.FromArgb(alpha, 253, 196, 39),
                    Color.FromArgb(alpha, 252, 204, 37),
                    Color.FromArgb(alpha, 250, 214, 36),
                    Color.FromArgb(alpha, 248, 223, 36),
                    Color.FromArgb(alpha, 245, 231, 38),
                    Color.FromArgb(alpha, 242, 240, 38),
                    Color.FromArgb(alpha, 239, 248, 33),
                };

                return colorRange;
            }

        }

        private static class Viridis
        {
            public static Color[] GetColors(int alpha = 255)
            {
                Color[] colorRange = {
                    Color.FromArgb(alpha, 68, 1, 84),
                    Color.FromArgb(alpha, 69, 8, 91),
                    Color.FromArgb(alpha, 71, 15, 98),
                    Color.FromArgb(alpha, 71, 22, 105),
                    Color.FromArgb(alpha, 72, 29, 111),
                    Color.FromArgb(alpha, 71, 37, 117),
                    Color.FromArgb(alpha, 71, 43, 122),
                    Color.FromArgb(alpha, 70, 49, 126),
                    Color.FromArgb(alpha, 68, 55, 129),
                    Color.FromArgb(alpha, 66, 62, 133),
                    Color.FromArgb(alpha, 64, 68, 135),
                    Color.FromArgb(alpha, 61, 74, 137),
                    Color.FromArgb(alpha, 59, 80, 138),
                    Color.FromArgb(alpha, 57, 85, 139),
                    Color.FromArgb(alpha, 54, 91, 140),
                    Color.FromArgb(alpha, 51, 96, 141),
                    Color.FromArgb(alpha, 49, 101, 141),
                    Color.FromArgb(alpha, 47, 106, 141),
                    Color.FromArgb(alpha, 44, 112, 142),
                    Color.FromArgb(alpha, 42, 117, 142),
                    Color.FromArgb(alpha, 40, 122, 142),
                    Color.FromArgb(alpha, 39, 126, 142),
                    Color.FromArgb(alpha, 37, 131, 141),
                    Color.FromArgb(alpha, 35, 137, 141),
                    Color.FromArgb(alpha, 33, 141, 140),
                    Color.FromArgb(alpha, 31, 146, 140),
                    Color.FromArgb(alpha, 30, 151, 138),
                    Color.FromArgb(alpha, 30, 156, 137),
                    Color.FromArgb(alpha, 31, 161, 135),
                    Color.FromArgb(alpha, 33, 166, 133),
                    Color.FromArgb(alpha, 36, 170, 130),
                    Color.FromArgb(alpha, 41, 175, 127),
                    Color.FromArgb(alpha, 48, 180, 122),
                    Color.FromArgb(alpha, 56, 185, 118),
                    Color.FromArgb(alpha, 64, 189, 114),
                    Color.FromArgb(alpha, 73, 193, 109),
                    Color.FromArgb(alpha, 85, 198, 102),
                    Color.FromArgb(alpha, 96, 201, 96),
                    Color.FromArgb(alpha, 107, 205, 89),
                    Color.FromArgb(alpha, 119, 208, 82),
                    Color.FromArgb(alpha, 131, 211, 75),
                    Color.FromArgb(alpha, 146, 215, 65),
                    Color.FromArgb(alpha, 159, 217, 56),
                    Color.FromArgb(alpha, 173, 220, 48),
                    Color.FromArgb(alpha, 186, 222, 39),
                    Color.FromArgb(alpha, 202, 224, 30),
                    Color.FromArgb(alpha, 215, 226, 25),
                    Color.FromArgb(alpha, 228, 227, 24),
                    Color.FromArgb(alpha, 241, 229, 28),
                    Color.FromArgb(alpha, 253, 231, 36),
                };
                return colorRange;
            }

        }

        #endregion

        #region Sequential colormaps
        private static class Blues
        {
            public static Color[] GetColors(int alpha = 255)
            {
                Color[] colorRange = {
                    Color.FromArgb(alpha, 247, 251, 255),
                    Color.FromArgb(alpha, 243, 248, 253),
                    Color.FromArgb(alpha, 239, 245, 252),
                    Color.FromArgb(alpha, 235, 243, 251),
                    Color.FromArgb(alpha, 231, 240, 249),
                    Color.FromArgb(alpha, 226, 237, 248),
                    Color.FromArgb(alpha, 222, 235, 247),
                    Color.FromArgb(alpha, 218, 232, 245),
                    Color.FromArgb(alpha, 215, 230, 244),
                    Color.FromArgb(alpha, 210, 227, 243),
                    Color.FromArgb(alpha, 206, 224, 241),
                    Color.FromArgb(alpha, 203, 222, 240),
                    Color.FromArgb(alpha, 199, 219, 239),
                    Color.FromArgb(alpha, 193, 217, 237),
                    Color.FromArgb(alpha, 186, 214, 234),
                    Color.FromArgb(alpha, 180, 211, 232),
                    Color.FromArgb(alpha, 173, 208, 230),
                    Color.FromArgb(alpha, 167, 206, 228),
                    Color.FromArgb(alpha, 160, 202, 225),
                    Color.FromArgb(alpha, 152, 199, 223),
                    Color.FromArgb(alpha, 144, 194, 222),
                    Color.FromArgb(alpha, 136, 190, 220),
                    Color.FromArgb(alpha, 128, 185, 218),
                    Color.FromArgb(alpha, 119, 180, 216),
                    Color.FromArgb(alpha, 111, 176, 214),
                    Color.FromArgb(alpha, 103, 171, 212),
                    Color.FromArgb(alpha, 97, 167, 210),
                    Color.FromArgb(alpha, 89, 162, 207),
                    Color.FromArgb(alpha, 83, 157, 204),
                    Color.FromArgb(alpha, 76, 153, 202),
                    Color.FromArgb(alpha, 70, 148, 199),
                    Color.FromArgb(alpha, 64, 144, 197),
                    Color.FromArgb(alpha, 58, 138, 193),
                    Color.FromArgb(alpha, 52, 132, 191),
                    Color.FromArgb(alpha, 47, 127, 188),
                    Color.FromArgb(alpha, 42, 122, 185),
                    Color.FromArgb(alpha, 36, 116, 182),
                    Color.FromArgb(alpha, 31, 111, 179),
                    Color.FromArgb(alpha, 27, 106, 175),
                    Color.FromArgb(alpha, 23, 101, 171),
                    Color.FromArgb(alpha, 19, 96, 167),
                    Color.FromArgb(alpha, 15, 90, 163),
                    Color.FromArgb(alpha, 11, 85, 159),
                    Color.FromArgb(alpha, 8, 80, 154),
                    Color.FromArgb(alpha, 8, 74, 146),
                    Color.FromArgb(alpha, 8, 68, 137),
                    Color.FromArgb(alpha, 8, 63, 130),
                    Color.FromArgb(alpha, 8, 58, 122),
                    Color.FromArgb(alpha, 8, 53, 114),
                    Color.FromArgb(alpha, 8, 48, 107),
                };

                return colorRange;
            }

        }

        private static class BuGn
        {
            public static Color[] GetColors(int alpha)
            {
                Color[] colorRange = {
                        Color.FromArgb(alpha, 247, 252, 253),
                        Color.FromArgb(alpha, 244, 250, 252),
                        Color.FromArgb(alpha, 241, 249, 251),
                        Color.FromArgb(alpha, 238, 248, 251),
                        Color.FromArgb(alpha, 235, 247, 250),
                        Color.FromArgb(alpha, 232, 246, 249),
                        Color.FromArgb(alpha, 229, 245, 249),
                        Color.FromArgb(alpha, 225, 243, 246),
                        Color.FromArgb(alpha, 221, 242, 243),
                        Color.FromArgb(alpha, 217, 240, 239),
                        Color.FromArgb(alpha, 213, 239, 237),
                        Color.FromArgb(alpha, 209, 237, 234),
                        Color.FromArgb(alpha, 205, 236, 231),
                        Color.FromArgb(alpha, 198, 233, 227),
                        Color.FromArgb(alpha, 189, 230, 221),
                        Color.FromArgb(alpha, 181, 227, 217),
                        Color.FromArgb(alpha, 173, 223, 212),
                        Color.FromArgb(alpha, 165, 220, 207),
                        Color.FromArgb(alpha, 155, 217, 202),
                        Color.FromArgb(alpha, 147, 213, 197),
                        Color.FromArgb(alpha, 139, 210, 191),
                        Color.FromArgb(alpha, 131, 206, 185),
                        Color.FromArgb(alpha, 123, 203, 179),
                        Color.FromArgb(alpha, 114, 199, 172),
                        Color.FromArgb(alpha, 106, 195, 166),
                        Color.FromArgb(alpha, 99, 192, 160),
                        Color.FromArgb(alpha, 93, 189, 153),
                        Color.FromArgb(alpha, 86, 185, 144),
                        Color.FromArgb(alpha, 80, 182, 137),
                        Color.FromArgb(alpha, 74, 179, 130),
                        Color.FromArgb(alpha, 68, 176, 122),
                        Color.FromArgb(alpha, 63, 172, 115),
                        Color.FromArgb(alpha, 57, 165, 106),
                        Color.FromArgb(alpha, 53, 160, 98),
                        Color.FromArgb(alpha, 48, 154, 90),
                        Color.FromArgb(alpha, 43, 149, 83),
                        Color.FromArgb(alpha, 38, 142, 73),
                        Color.FromArgb(alpha, 33, 137, 67),
                        Color.FromArgb(alpha, 27, 132, 63),
                        Color.FromArgb(alpha, 22, 127, 59),
                        Color.FromArgb(alpha, 16, 123, 55),
                        Color.FromArgb(alpha, 10, 117, 51),
                        Color.FromArgb(alpha, 4, 112, 47),
                        Color.FromArgb(alpha, 0, 107, 43),
                        Color.FromArgb(alpha, 0, 101, 40),
                        Color.FromArgb(alpha, 0, 93, 37),
                        Color.FromArgb(alpha, 0, 87, 35),
                        Color.FromArgb(alpha, 0, 80, 32),
                        Color.FromArgb(alpha, 0, 74, 29),
                        Color.FromArgb(alpha, 0, 68, 27)};
                return colorRange;
            }
        }

        private static class YlGnBu
        {
            public static Color[] GetColors(int alpha)
            {
                Color[] colorRange = {
                        Color.FromArgb(alpha, 255, 255, 217),
                        Color.FromArgb(alpha, 252, 253, 210),
                        Color.FromArgb(alpha, 249, 252, 204),
                        Color.FromArgb(alpha, 246, 251, 198),
                        Color.FromArgb(alpha, 243, 250, 191),
                        Color.FromArgb(alpha, 240, 249, 184),
                        Color.FromArgb(alpha, 237, 248, 178),
                        Color.FromArgb(alpha, 232, 246, 177),
                        Color.FromArgb(alpha, 226, 243, 177),
                        Color.FromArgb(alpha, 218, 240, 178),
                        Color.FromArgb(alpha, 213, 238, 178),
                        Color.FromArgb(alpha, 207, 236, 179),
                        Color.FromArgb(alpha, 201, 233, 179),
                        Color.FromArgb(alpha, 191, 230, 180),
                        Color.FromArgb(alpha, 178, 224, 182),
                        Color.FromArgb(alpha, 166, 220, 183),
                        Color.FromArgb(alpha, 155, 216, 184),
                        Color.FromArgb(alpha, 144, 211, 185),
                        Color.FromArgb(alpha, 130, 206, 186),
                        Color.FromArgb(alpha, 120, 202, 187),
                        Color.FromArgb(alpha, 110, 198, 189),
                        Color.FromArgb(alpha, 100, 195, 190),
                        Color.FromArgb(alpha, 91, 191, 192),
                        Color.FromArgb(alpha, 79, 187, 193),
                        Color.FromArgb(alpha, 69, 183, 195),
                        Color.FromArgb(alpha, 62, 179, 195),
                        Color.FromArgb(alpha, 56, 173, 195),
                        Color.FromArgb(alpha, 49, 166, 194),
                        Color.FromArgb(alpha, 44, 160, 193),
                        Color.FromArgb(alpha, 38, 154, 193),
                        Color.FromArgb(alpha, 32, 148, 192),
                        Color.FromArgb(alpha, 29, 142, 190),
                        Color.FromArgb(alpha, 30, 132, 186),
                        Color.FromArgb(alpha, 30, 124, 182),
                        Color.FromArgb(alpha, 31, 116, 178),
                        Color.FromArgb(alpha, 32, 108, 174),
                        Color.FromArgb(alpha, 33, 99, 170),
                        Color.FromArgb(alpha, 34, 91, 166),
                        Color.FromArgb(alpha, 34, 85, 163),
                        Color.FromArgb(alpha, 35, 78, 160),
                        Color.FromArgb(alpha, 35, 71, 157),
                        Color.FromArgb(alpha, 36, 64, 153),
                        Color.FromArgb(alpha, 36, 57, 150),
                        Color.FromArgb(alpha, 36, 51, 146),
                        Color.FromArgb(alpha, 31, 47, 136),
                        Color.FromArgb(alpha, 26, 43, 125),
                        Color.FromArgb(alpha, 21, 39, 116),
                        Color.FromArgb(alpha, 17, 36, 106),
                        Color.FromArgb(alpha, 12, 32, 97),
                        Color.FromArgb(alpha, 8, 29, 88)};
                return colorRange;
            }
        }
        #endregion

        #region Diverging colormaps
        private static class CoolWarm
        {
            public static Color[] GetColors(int alpha = 255)
            {
                Color[] colorRange = {
                    Color.FromArgb(alpha, 58, 76, 192),
                    Color.FromArgb(alpha, 64, 84, 199),
                    Color.FromArgb(alpha, 70, 93, 207),
                    Color.FromArgb(alpha, 76, 102, 214),
                    Color.FromArgb(alpha, 82, 110, 220),
                    Color.FromArgb(alpha, 90, 120, 227),
                    Color.FromArgb(alpha, 96, 128, 232),
                    Color.FromArgb(alpha, 103, 136, 237),
                    Color.FromArgb(alpha, 109, 144, 241),
                    Color.FromArgb(alpha, 117, 152, 246),
                    Color.FromArgb(alpha, 124, 160, 249),
                    Color.FromArgb(alpha, 131, 166, 251),
                    Color.FromArgb(alpha, 138, 173, 253),
                    Color.FromArgb(alpha, 145, 179, 254),
                    Color.FromArgb(alpha, 153, 186, 254),
                    Color.FromArgb(alpha, 160, 191, 254),
                    Color.FromArgb(alpha, 167, 196, 253),
                    Color.FromArgb(alpha, 174, 201, 252),
                    Color.FromArgb(alpha, 182, 206, 249),
                    Color.FromArgb(alpha, 188, 209, 246),
                    Color.FromArgb(alpha, 194, 212, 243),
                    Color.FromArgb(alpha, 200, 215, 239),
                    Color.FromArgb(alpha, 206, 217, 235),
                    Color.FromArgb(alpha, 213, 219, 229),
                    Color.FromArgb(alpha, 218, 220, 223),
                    Color.FromArgb(alpha, 223, 219, 217),
                    Color.FromArgb(alpha, 228, 216, 209),
                    Color.FromArgb(alpha, 233, 212, 201),
                    Color.FromArgb(alpha, 237, 208, 193),
                    Color.FromArgb(alpha, 240, 204, 185),
                    Color.FromArgb(alpha, 242, 199, 178),
                    Color.FromArgb(alpha, 244, 194, 170),
                    Color.FromArgb(alpha, 246, 187, 160),
                    Color.FromArgb(alpha, 247, 181, 152),
                    Color.FromArgb(alpha, 247, 174, 145),
                    Color.FromArgb(alpha, 246, 167, 137),
                    Color.FromArgb(alpha, 245, 158, 127),
                    Color.FromArgb(alpha, 243, 150, 120),
                    Color.FromArgb(alpha, 241, 142, 112),
                    Color.FromArgb(alpha, 238, 134, 105),
                    Color.FromArgb(alpha, 234, 125, 97),
                    Color.FromArgb(alpha, 230, 114, 89),
                    Color.FromArgb(alpha, 225, 104, 82),
                    Color.FromArgb(alpha, 220, 94, 75),
                    Color.FromArgb(alpha, 215, 84, 68),
                    Color.FromArgb(alpha, 207, 70, 61),
                    Color.FromArgb(alpha, 201, 59, 55),
                    Color.FromArgb(alpha, 194, 45, 49),
                    Color.FromArgb(alpha, 187, 26, 43),
                    Color.FromArgb(alpha, 179, 3, 38),
                };

                return colorRange;
            }

        }

        #endregion

        #region Sequential 2-colormaps
        private static class Cool
        {
            public static Color[] GetColors(int alpha = 255)
            {
                Color[] colorRange = {
                    Color.FromArgb(alpha, 0, 255, 255),
                    Color.FromArgb(alpha, 5, 250, 255),
                    Color.FromArgb(alpha, 10, 245, 255),
                    Color.FromArgb(alpha, 15, 240, 255),
                    Color.FromArgb(alpha, 20, 235, 255),
                    Color.FromArgb(alpha, 26, 229, 255),
                    Color.FromArgb(alpha, 31, 224, 255),
                    Color.FromArgb(alpha, 36, 219, 255),
                    Color.FromArgb(alpha, 40, 214, 255),
                    Color.FromArgb(alpha, 47, 208, 255),
                    Color.FromArgb(alpha, 52, 203, 255),
                    Color.FromArgb(alpha, 56, 198, 255),
                    Color.FromArgb(alpha, 62, 193, 255),
                    Color.FromArgb(alpha, 67, 188, 255),
                    Color.FromArgb(alpha, 73, 182, 255),
                    Color.FromArgb(alpha, 78, 177, 255),
                    Color.FromArgb(alpha, 83, 172, 255),
                    Color.FromArgb(alpha, 88, 167, 255),
                    Color.FromArgb(alpha, 94, 161, 255),
                    Color.FromArgb(alpha, 99, 156, 255),
                    Color.FromArgb(alpha, 104, 151, 255),
                    Color.FromArgb(alpha, 109, 146, 255),
                    Color.FromArgb(alpha, 113, 141, 255),
                    Color.FromArgb(alpha, 120, 135, 255),
                    Color.FromArgb(alpha, 125, 130, 255),
                    Color.FromArgb(alpha, 130, 125, 255),
                    Color.FromArgb(alpha, 135, 120, 255),
                    Color.FromArgb(alpha, 141, 113, 255),
                    Color.FromArgb(alpha, 146, 109, 255),
                    Color.FromArgb(alpha, 151, 104, 255),
                    Color.FromArgb(alpha, 156, 98, 255),
                    Color.FromArgb(alpha, 161, 94, 255),
                    Color.FromArgb(alpha, 167, 88, 255),
                    Color.FromArgb(alpha, 172, 82, 255),
                    Color.FromArgb(alpha, 177, 78, 255),
                    Color.FromArgb(alpha, 182, 73, 255),
                    Color.FromArgb(alpha, 188, 66, 255),
                    Color.FromArgb(alpha, 193, 62, 255),
                    Color.FromArgb(alpha, 198, 56, 255),
                    Color.FromArgb(alpha, 203, 52, 255),
                    Color.FromArgb(alpha, 208, 47, 255),
                    Color.FromArgb(alpha, 214, 40, 255),
                    Color.FromArgb(alpha, 219, 36, 255),
                    Color.FromArgb(alpha, 224, 31, 255),
                    Color.FromArgb(alpha, 229, 25, 255),
                    Color.FromArgb(alpha, 235, 20, 255),
                    Color.FromArgb(alpha, 240, 15, 255),
                    Color.FromArgb(alpha, 245, 9, 255),
                    Color.FromArgb(alpha, 250, 5, 255),
                    Color.FromArgb(alpha, 255, 0, 255) };
                return colorRange;

            }
        }

        #endregion

        #region Miscellaneous colormaps
        private static class Turbo
        {
            public static Color[] GetColors(int alpha = 255)
            {
                Color[] colorRange = {
                        Color.FromArgb(alpha, 48, 18, 59),
                        Color.FromArgb(alpha, 54, 33, 95),
                        Color.FromArgb(alpha, 59, 47, 127),
                        Color.FromArgb(alpha, 63, 61, 156),
                        Color.FromArgb(alpha, 66, 75, 181),
                        Color.FromArgb(alpha, 69, 91, 206),
                        Color.FromArgb(alpha, 70, 104, 224),
                        Color.FromArgb(alpha, 70, 117, 237),
                        Color.FromArgb(alpha, 70, 130, 248),
                        Color.FromArgb(alpha, 66, 145, 254),
                        Color.FromArgb(alpha, 60, 157, 253),
                        Color.FromArgb(alpha, 52, 170, 248),
                        Color.FromArgb(alpha, 43, 182, 239),
                        Color.FromArgb(alpha, 35, 194, 228),
                        Color.FromArgb(alpha, 27, 207, 212),
                        Color.FromArgb(alpha, 23, 217, 199),
                        Color.FromArgb(alpha, 24, 225, 186),
                        Color.FromArgb(alpha, 30, 232, 175),
                        Color.FromArgb(alpha, 44, 239, 157),
                        Color.FromArgb(alpha, 59, 244, 141),
                        Color.FromArgb(alpha, 77, 249, 124),
                        Color.FromArgb(alpha, 97, 252, 108),
                        Color.FromArgb(alpha, 116, 254, 92),
                        Color.FromArgb(alpha, 139, 254, 75),
                        Color.FromArgb(alpha, 155, 253, 64),
                        Color.FromArgb(alpha, 169, 251, 57),
                        Color.FromArgb(alpha, 182, 247, 53),
                        Color.FromArgb(alpha, 197, 239, 51),
                        Color.FromArgb(alpha, 209, 232, 52),
                        Color.FromArgb(alpha, 221, 224, 54),
                        Color.FromArgb(alpha, 231, 215, 56),
                        Color.FromArgb(alpha, 239, 205, 57),
                        Color.FromArgb(alpha, 247, 192, 57),
                        Color.FromArgb(alpha, 251, 181, 55),
                        Color.FromArgb(alpha, 253, 169, 50),
                        Color.FromArgb(alpha, 254, 155, 45),
                        Color.FromArgb(alpha, 252, 137, 38),
                        Color.FromArgb(alpha, 250, 122, 31),
                        Color.FromArgb(alpha, 246, 107, 24),
                        Color.FromArgb(alpha, 241, 93, 19),
                        Color.FromArgb(alpha, 234, 80, 13),
                        Color.FromArgb(alpha, 226, 66, 9),
                        Color.FromArgb(alpha, 217, 56, 6),
                        Color.FromArgb(alpha, 208, 47, 4),
                        Color.FromArgb(alpha, 197, 38, 2),
                        Color.FromArgb(alpha, 182, 28, 1),
                        Color.FromArgb(alpha, 169, 21, 1),
                        Color.FromArgb(alpha, 154, 14, 1),
                        Color.FromArgb(alpha, 139, 9, 1),
                        Color.FromArgb(alpha, 122, 4, 2)};
                return colorRange;
            }
            #endregion
        }
    }
}



