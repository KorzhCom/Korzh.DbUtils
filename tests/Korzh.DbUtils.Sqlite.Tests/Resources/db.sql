PRAGMA foreign_keys=ON;
CREATE TABLE "Categories" (
    "CategoryID" INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY,
    "CategoryName" TEXT NULL,
    "Description" TEXT NULL,
    "Picture" BLOB NULL
);
CREATE TABLE "Customers" (
    "CustomerID" TEXT NOT NULL CONSTRAINT "PK_Customers" PRIMARY KEY,
    "CompanyName" TEXT NULL,
    "Address" TEXT NULL,
    "City" TEXT NULL,
    "Region" TEXT NULL,
    "PostalCode" TEXT NULL,
    "Country" TEXT NULL,
    "ContactName" TEXT NULL,
    "ContactTitle" TEXT NULL,
    "Phone" TEXT NULL,
    "Fax" TEXT NULL,
    "RowVersion" BLOB NULL
);
CREATE TABLE "Employees" (
    "EmployeeID" INTEGER NOT NULL CONSTRAINT "PK_Employees" PRIMARY KEY,
    "LastName" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "Title" TEXT NULL,
    "TitleOfCourtesy" TEXT NULL,
    "BirthDate" TEXT NULL,
    "HireDate" TEXT NULL,
    "Address" TEXT NULL,
    "City" TEXT NULL,
    "Region" TEXT NULL,
    "PostalCode" TEXT NULL,
    "Country" TEXT NULL,
    "HomePhone" TEXT NULL,
    "Extension" TEXT NULL,
    "Photo" BLOB NULL,
    "PhotoPath" TEXT NULL,
    "Notes" TEXT NULL,
    "ReportsTo" INTEGER NULL,
    CONSTRAINT "FK_Employees_Employees_ReportsTo" FOREIGN KEY ("ReportsTo") REFERENCES "Employees" ("EmployeeID") ON DELETE RESTRICT
);
CREATE TABLE "Shippers" (
    "SupplierID" INTEGER NOT NULL CONSTRAINT "PK_Shippers" PRIMARY KEY AUTOINCREMENT,
    "CompanyName" TEXT NULL,
    "Phone" TEXT NULL
);
CREATE TABLE "Suppliers" (
    "SupplierID" INTEGER NOT NULL CONSTRAINT "PK_Suppliers" PRIMARY KEY AUTOINCREMENT,
    "CompanyName" TEXT NULL,
    "ContactName" TEXT NULL,
    "ContactTitle" TEXT NULL,
    "Address" TEXT NULL,
    "City" TEXT NULL,
    "Region" TEXT NULL,
    "PostalCode" TEXT NULL,
    "Country" TEXT NULL,
    "Phone" TEXT NULL,
    "Fax" TEXT NULL,
    "HomePage" TEXT NULL
);
CREATE TABLE "Orders" (
    "OrderID" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY,
    "OrderDate" TEXT NULL,
    "RequiredDate" TEXT NULL,
    "ShippedDate" TEXT NULL,
    "Freight" TEXT NULL,
    "CustomerID" TEXT NULL,
    "EmployeeID" INTEGER NULL,
    "ShipVia" INTEGER NULL,
    "ShipName" TEXT NULL,
    "ShipAddress" TEXT NULL,
    "ShipCity" TEXT NULL,
    "ShipRegion" TEXT NULL,
    "ShipPostalCode" TEXT NULL,
    "ShipCountry" TEXT NULL,
    "RowVersion" BLOB NULL,
    CONSTRAINT "FK_Orders_Customers_CustomerID" FOREIGN KEY ("CustomerID") REFERENCES "Customers" ("CustomerID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Employees_EmployeeID" FOREIGN KEY ("EmployeeID") REFERENCES "Employees" ("EmployeeID") ON DELETE RESTRICT
);
CREATE TABLE "Products" (
    "ProductID" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY,
    "ProductName" TEXT NULL,
    "SupplierID" INTEGER NULL,
    "CategoryID" INTEGER NULL,
    "QuantityPerUnit" TEXT NULL,
    "UnitPrice" TEXT NULL,
    "UnitsInStock" INTEGER NULL,
    "UnitsOnOrder" INTEGER NULL,
    "ReorderLevel" INTEGER NULL,
    "Discontinued" INTEGER NOT NULL,
    CONSTRAINT "FK_Products_Categories_CategoryID" FOREIGN KEY ("CategoryID") REFERENCES "Categories" ("CategoryID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Products_Suppliers_SupplierID" FOREIGN KEY ("SupplierID") REFERENCES "Suppliers" ("SupplierID") ON DELETE RESTRICT
);
CREATE TABLE "Order_Details" (
    "OrderID" INTEGER NOT NULL,
    "ProductID" INTEGER NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "Discount" REAL NOT NULL,
    CONSTRAINT "PK_Order_Details" PRIMARY KEY ("OrderID", "ProductID"),
    CONSTRAINT "FK_Order_Details_Orders_OrderID" FOREIGN KEY ("OrderID") REFERENCES "Orders" ("OrderID") ON DELETE CASCADE,
    CONSTRAINT "FK_Order_Details_Products_ProductID" FOREIGN KEY ("ProductID") REFERENCES "Products" ("ProductID") ON DELETE CASCADE
);
CREATE INDEX "IX_Employees_ReportsTo" ON "Employees" ("ReportsTo");
CREATE INDEX "IX_Order_Details_ProductID" ON "Order_Details" ("ProductID");
CREATE INDEX "IX_Orders_CustomerID" ON "Orders" ("CustomerID");
CREATE INDEX "IX_Orders_EmployeeID" ON "Orders" ("EmployeeID");
CREATE INDEX "IX_Products_CategoryID" ON "Products" ("CategoryID");
CREATE INDEX "IX_Products_SupplierID" ON "Products" ("SupplierID");