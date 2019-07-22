cd src
cd Korzh.DbUtils
dotnet publish --configuration=Release --output=c:\Projects\KCL\Korzh.DbUtils\publish
cd ..

cd Korzh.DbUtils.Import
dotnet publish --configuration=Release --output=c:\Projects\KCL\Korzh.DbUtils\publish
cd ..

cd Korzh.DbUtils.Export
dotnet publish --configuration=Release --output=c:\Projects\KCL\Korzh.DbUtils\publish
cd ..

cd Korzh.DbUtils.MySql
dotnet publish --configuration=Release --output=c:\Projects\KCL\Korzh.DbUtils\publish
cd ..

cd Korzh.DbUtils.SqlServer
dotnet publish --configuration=Release --output=c:\Projects\KCL\Korzh.DbUtils\publish
cd ..

:exit

cd ..
