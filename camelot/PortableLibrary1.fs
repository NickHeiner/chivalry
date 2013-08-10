namespace camelot

module camelot =
    let IsPrime n =
            if n < 2L then
                false
            else
                seq { 2L .. n - 1L}
                |> Seq.exists(fun x -> n % x = 0L)
                |> not