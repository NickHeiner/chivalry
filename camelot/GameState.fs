namespace Camelot

module GameState =

    type row = Row of int
    type col = Col of int
    type coord = {row : row; col : col}
    type team = Recepient| Initiator
    type piece = Tall | Short
    type boardSpaceState = (team * piece)
    type game = {pieceLocations: Map<coord, boardSpaceState option>}

module GameStateUtils =

    open GameState

    let IsJumpable _ src toJump = src.row > toJump.row

