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

    public PlayerModel ParseFromJson(SimpleJSON.JSONNode json) {
        return new PlayerModel(
            json["user_id"].AsInt,
            json["user_nickname"],
            json["user_login"],
            json["user_money_amount"].AsInt,
            json["user_present_theme"],
            json["user_match_victory"].AsInt,
            json["user_match_defeat"].AsInt,
            json["user_match_boats_sunk"].AsInt
        );
    }
}
