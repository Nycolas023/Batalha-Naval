public class PlayerModel {

    public int User_Id { get; set; }
    public string User_Nickname { get; set; }
    public string User_Login { get; set; }
    public int User_Money_Amount { get; set; }
    public string User_Present_Theme { get; set; }
    public int User_Match_Victory { get; set; }
    public int User_Match_Defeat { get; set; }
    public int User_Match_Boats_Sunk { get; set; }

    public PlayerModel(int userId, string userNickname, string userLogin, int userMoneyAmount, string userPresentTheme,
                       int userMatchVictory, int userMatchDefeat, int userMatchBoatsSunk) {
        User_Id = userId;
        User_Nickname = userNickname;
        User_Login = userLogin;
        User_Money_Amount = userMoneyAmount;
        User_Present_Theme = userPresentTheme;
        User_Match_Victory = userMatchVictory;
        User_Match_Defeat = userMatchDefeat;
        User_Match_Boats_Sunk = userMatchBoatsSunk;
    }
}
