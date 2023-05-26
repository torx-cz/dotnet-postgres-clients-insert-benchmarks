#shell.nix
let
    pkgs = import (builtins.fetchTarball {
        url = "https://github.com/NixOS/nixpkgs/archive/ab1254087f4cdf4af74b552d7fc95175d9bdbb49.tar.gz";   # nixpkgs - 2023-01-22
    }) {};

in

with pkgs;

mkShell {
  name = "dotnet-env";
  packages = [
    dotnet-sdk_7
  ];
}
