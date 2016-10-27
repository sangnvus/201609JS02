Setup: 
- Mo project
- Phan TOOLS tren thanh Taskbar -> Library Package Manager -> Package Manage Console
- Trong bang Console nhap: Install-Package AspNet.Identity.MongoDB
- Chay xong tiep tuc nhap trong Console: Install-Package Microsoft.Owin
- Cho chac chan thi nen nhap tiep: Install-Package mongocsharpdriver 
	va Install-Package Microsoft.AspNet.Identity.Owin
- Sau khi Install cac Package thi Clear Solution va Rebuilt
- Neu bi loi "The type or namespace name 'Mvc/Razor/...' does not exist " 
	thi chay tiep Install-Package Microsoft.AspNet.Mvc
 :) Hy vong ko bi loi 