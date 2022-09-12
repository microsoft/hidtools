:: Release build must have been previously run.

nuget pack HidSpecification\HidSpecification.csproj -Properties Configuration=Release -Properties NoWarn=NU5128

nuget pack HidEngine\HidEngine.csproj -Properties Configuration=Release -Properties NoWarn=NU5128