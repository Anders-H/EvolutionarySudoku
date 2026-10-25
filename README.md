# Evolutionary Sudoku

An evolutionary algorithm that generates a sudoku.

Each run gives a 9x9 number grid in the console window.

Run the generator in Release mode:

```sh
dotnet run --project EvolutionarySudoku -c Release
```

Run the dependency-free score regression checks:

```sh
dotnet run --project EvolutionarySudoku.Checks -c Release
```

The checks compare scoring with an independent per-cell reference on 10,000
seeded random grids, a valid grid, and an off-diagonal box conflict.

## Sample result

```
193452867
842967153
675831942
756298431
284315679
931674285
329586714
418723596
567149328
```
