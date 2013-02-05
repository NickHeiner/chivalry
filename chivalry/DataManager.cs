using chivalry.Controllers;
using chivalry.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace chivalry
{
    public class DataManager
    {
        private IMobileServiceTable<Game> gameTable = App.MobileService.GetTable<Game>();

        public async Task<User> withServerData(User user)
        {
            user = user ?? new User();

            if (((App)Application.Current).OFFLINE_MODE)
            {
                user.Email = "nick_heiner@hotmail.com";
                user.ProfilePicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic";

                Game againstScott = new Game() { RecepientPlayerName = "Scott", InitiatingPlayerEmail = user.Email, WaitingOn = AbsolutePlayer.Recepient };
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 5 }, BoardSpaceState.FriendlyPieceShort);
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 6 }, BoardSpaceState.FriendlyPieceTall);
                againstScott.SetPieceLocation(new Coord() { Row = 4, Col = 4 }, BoardSpaceState.OpponentPieceShort);
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 8 }, BoardSpaceState.OpponentPieceTall);

                user.Games.Add(againstScott);

                var dadGame = GameController.WithStartingPieces(new Game()
                {
                    WaitingOn = AbsolutePlayer.Initiator,
                    RecepientPlayerName = "Dad",
                    InitiatingPlayerEmail = user.Email,
                    InitiaitingPlayerPicSource = user.ProfilePicSource,
                    RecepientPlayerPicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic"
                });

                updateWithUserData(dadGame, user);

                user.Games.Add(dadGame);

                var wonGame = GameController.WithStartingPieces(new Game()
                {
                    WaitingOn = AbsolutePlayer.Initiator,
                    RecepientPlayerName = "Losing Guy",
                    InitiatingPlayerEmail = user.Email,
                    InitiaitingPlayerPicSource = user.ProfilePicSource,
                    RecepientPlayerPicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic"
                });

                wonGame.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_1), BoardSpaceState.FriendlyPieceShort);
                wonGame.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_2), BoardSpaceState.FriendlyPieceShort);

                updateWithUserData(wonGame, user);

                user.Games.Add(wonGame);

                var lostGame = GameController.WithStartingPieces(new Game()
                {
                    WaitingOn = AbsolutePlayer.Recepient,
                    RecepientPlayerName = "Winning Guy",
                    InitiatingPlayerEmail = user.Email,
                    InitiaitingPlayerPicSource = user.ProfilePicSource,
                    RecepientPlayerPicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic"
                });

                lostGame.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_1), BoardSpaceState.OpponentPieceShort);
                lostGame.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_2), BoardSpaceState.OpponentPieceShort);

                updateWithUserData(lostGame, user);

                user.Games.Add(lostGame);

                return user;
            }

            user.Games.Clear();
            var games = await gameTable.ToListAsync();
            foreach (var game in games)
            {
                if (((App)Application.Current).DEMO_HACK)
                {
                    GameController.WithStartingPieces(game);
                    foreach (var _ in Enumerable.Range(0, 10))
                    {
                        game.CapturePiece(BoardSpaceState.OpponentPieceShort);
                        game.CapturePiece(BoardSpaceState.FriendlyPieceShort);
                    }
                    foreach (var _ in Enumerable.Range(0, 4))
                    {
                        game.CapturePiece(BoardSpaceState.OpponentPieceTall);
                        game.CapturePiece(BoardSpaceState.FriendlyPieceTall);
                    }
                }
                updateWithUserData(game, user);
                user.Games.Add(game);
            }

            return user;
        }

        // visible for testing
        public void updateWithUserData(Game game, User user)
        {
            game.Transformation = game.RecepientPlayerEmail == user.Email ? BoardCoord.Transformation.FLIP : BoardCoord.Transformation.NO_FLIP;
        }

        internal async void AddNewGame(User user, string recepientUserName, string recepientUserEmail)
        {
            await gameTable.InsertAsync(GameController.WithStartingPieces(new Game() 
            { 
                InitiatingPlayerName = user.Name,
                InitiatingPlayerEmail = user.Email,
                InitiaitingPlayerPicSource = user.ProfilePicSource,
                RecepientPlayerName = recepientUserName, 
                RecepientPlayerEmail = recepientUserEmail,

                // TODO http://stackoverflow.com/questions/14677744/get-contact-thumbnail
                RecepientPlayerPicSource = ""
            }));
        }

    }
}
