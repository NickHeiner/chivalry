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

                Game againstScott = new Game() 
                { 
                    RecepientPlayerName = "Scott", 
                    InitiatingPlayerEmail = user.Email, 
                    WaitingOn = AbsolutePlayer.Recepient,
                    LastMoveSubmittedAt = DateTime.Now - TimeSpan.FromDays(1)
                };
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 5 }, BoardSpaceState.InitiatorPieceShort);
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 6 }, BoardSpaceState.InitiatorPieceTall);
                againstScott.SetPieceLocation(new Coord() { Row = 4, Col = 4 }, BoardSpaceState.RecepientPieceShort);
                againstScott.SetPieceLocation(new Coord() { Row = 5, Col = 8 }, BoardSpaceState.RecepientPieceTall);

                user.Games.Add(againstScott);

                var dadGame = GameController.WithStartingPieces(new Game()
                {
                    WaitingOn = AbsolutePlayer.Initiator,
                    RecepientPlayerName = "Dad",
                    InitiatingPlayerEmail = user.Email,
                    InitiaitingPlayerPicSource = user.ProfilePicSource,
                    RecepientPlayerPicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic",
                    LastMoveSubmittedAt = DateTime.Now - TimeSpan.FromHours(2.3)
                });

                user.Games.Add(dadGame);

                var wonGame = GameController.WithStartingPieces(new Game()
                {
                    WaitingOn = AbsolutePlayer.Initiator,
                    RecepientPlayerName = "Losing Guy",
                    InitiatingPlayerEmail = user.Email,
                    InitiaitingPlayerPicSource = user.ProfilePicSource,
                    RecepientPlayerPicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic",
                    LastMoveSubmittedAt = DateTime.Now - TimeSpan.FromDays(15.4)
                });

                wonGame.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_1), BoardSpaceState.InitiatorPieceShort);
                wonGame.SetPieceLocation(Coord.Create(0, Game.ENDZONE_COL_2), BoardSpaceState.InitiatorPieceShort);

                user.Games.Add(wonGame);

                var lostGame = GameController.WithStartingPieces(new Game()
                {
                    WaitingOn = AbsolutePlayer.Recepient,
                    RecepientPlayerName = "Winning Guy",
                    InitiatingPlayerEmail = user.Email,
                    InitiaitingPlayerPicSource = user.ProfilePicSource,
                    RecepientPlayerPicSource = "https://cid-0c175b9b686f66fd.users.storage.live.com/users/0x0c175b9b686f66fd/myprofile/expressionprofile/profilephoto:UserTileStatic",
                    LastMoveSubmittedAt = DateTime.Now - TimeSpan.FromDays(23.9)
                });

                lostGame.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_1), BoardSpaceState.RecepientPieceShort);
                lostGame.SetPieceLocation(Coord.Create(Game.BOARD_ROW_MAX, Game.ENDZONE_COL_2), BoardSpaceState.RecepientPieceShort);

                user.Games.Add(lostGame);

                return user;
            }

            user.Games.Clear();
            var games = await 
                gameTable
                .Where(game => game.InitiatingPlayerEmail == user.Email || game.RecepientPlayerEmail == user.Email)
                .ToListAsync();
            foreach (var game in games)
            {
                if (((App)Application.Current).DEMO_HACK)
                {
                    GameController.WithStartingPieces(game);
                    foreach (var _ in Enumerable.Range(0, 10))
                    {
                        game.CapturePiece(BoardSpaceState.RecepientPieceShort);
                        game.CapturePiece(BoardSpaceState.InitiatorPieceShort);
                    }
                    foreach (var _ in Enumerable.Range(0, 4))
                    {
                        game.CapturePiece(BoardSpaceState.RecepientPieceTall);
                        game.CapturePiece(BoardSpaceState.InitiatorPieceTall);
                    }
                }
                GameController.SetOtherPlayerLabel(user, game);
                user.Games.Add(game);
            }

            return user;
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
                LastMoveSubmittedAt = DateTime.Now,

                // TODO http://stackoverflow.com/questions/14677744/get-contact-thumbnail
                RecepientPlayerPicSource = ""
            }));
        }

        internal void SaveGame(Game game)
        {
            gameTable.UpdateAsync(game);
        }
    }
}
