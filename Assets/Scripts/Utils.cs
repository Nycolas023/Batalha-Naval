public class Utils {
    public int[,] RotateMatrix(int[,] matrix) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[,] rotatedMatrix = new int[cols, rows];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                rotatedMatrix[j, rows - 1 - i] = matrix[i, j];
            }
        }

        return rotatedMatrix;
    }
}
