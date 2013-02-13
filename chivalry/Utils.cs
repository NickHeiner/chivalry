using chivalry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chivalry.Utils
{
    public static class Utils
    {
        public static RelativePlayer ToRelativePlayer(this AbsolutePlayer absolutePlayer, User user, Game game)
        {
            return user.Email == game.InitiatingPlayerEmail ?
                absolutePlayer == AbsolutePlayer.Initiator ? RelativePlayer.Friendly : RelativePlayer.Opponent :
                absolutePlayer == AbsolutePlayer.Initiator ? RelativePlayer.Opponent : RelativePlayer.Friendly;
        }

        public static AbsolutePlayer Toggle(this AbsolutePlayer absolutePlayer)
        {
            return absolutePlayer == AbsolutePlayer.Initiator ? AbsolutePlayer.Recepient : AbsolutePlayer.Initiator;
        }

        public static AbsolutePlayer ToAbsolutePlayer(this User user, Game game)
        {
            return user.Email == game.InitiatingPlayerEmail ? AbsolutePlayer.Initiator : AbsolutePlayer.Recepient;
        }

        public static BoardSpaceState Toggle(this BoardSpaceState boardSpaceState)
        {
            switch (boardSpaceState)
            {
                case BoardSpaceState.None:
                    return BoardSpaceState.None;
                case BoardSpaceState.OpponentPieceShort:
                    return BoardSpaceState.FriendlyPieceShort;
                case BoardSpaceState.OpponentPieceTall:
                    return BoardSpaceState.FriendlyPieceTall;
                case BoardSpaceState.FriendlyPieceShort:
                    return BoardSpaceState.OpponentPieceShort;
                case BoardSpaceState.FriendlyPieceTall:
                    return BoardSpaceState.OpponentPieceTall;
            }

            throw new InvalidOperationException();
        }

        // copied from https://gist.github.com/2463179
        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var previous = default(T);

            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    previous = enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    yield return Tuple.Create(previous, enumerator.Current);
                    previous = enumerator.Current;
                }
            }
        }
    }
}
