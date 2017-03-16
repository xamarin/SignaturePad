&.\ensureNugetInstalled.ps1

# delete all obj folders as temporary generated *.cs files often destroy the symbols package with "item was already added" error
get-childitem .. -include obj -recurse -force | remove-item -recurse -force

nuget pack SignaturePad.symbols.nuspec -symbols
nuget pack SignaturePad.Forms.symbols.nuspec -symbols
