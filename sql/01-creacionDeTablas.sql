CREATE TABLE Clientes (
    ClienteID INT IDENTITY(1,1) PRIMARY KEY,      
    Nombre NVARCHAR(100) NOT NULL,              
    Email NVARCHAR(100) NOT NULL,                     
    Telefono NVARCHAR(20) NOT NULL,                  
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()  
);

CREATE TABLE Criptomonedas (
    CryptoCode NVARCHAR(50) PRIMARY KEY, 
    Nombre NVARCHAR(100) NOT NULL,              
    TipoValor NVARCHAR(20) NULL -- "estable" o "volatil"
);

CREATE TABLE Exchanges (
    ExchangeID INT IDENTITY(1,1) PRIMARY KEY, 
    Nombre NVARCHAR(100) NOT NULL,           
    URL NVARCHAR(255) NULL,                  
    CodigoAPI NVARCHAR(50) NULL, -- Muchas APIs usan un código o nombre para identificar el exchange
);

CREATE TABLE Transacciones (
    TransaccionID INT IDENTITY(1,1) PRIMARY KEY,       
    ClienteID INT NOT NULL,                            
    CryptoCode NVARCHAR(50) NOT NULL,                 
    Accion NVARCHAR(20) NOT NULL, -- "purchase" o "sale" (compra o venta)
    cantidadCripto DECIMAL(18,8) NOT NULL,           
    monto DECIMAL(18,2) NOT NULL,-- Monto en pesos argentinos pagados o recibidos
    fecha DATETIME NOT NULL,                        
    ExchangeID INT NULL,-- Opcional: ID del exchange donde se hizo la operación
    CONSTRAINT FK_Transacciones_Clientes FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),
    CONSTRAINT FK_Transacciones_Criptomonedas FOREIGN KEY (CryptoCode) REFERENCES Criptomonedas(CryptoCode),
    CONSTRAINT FK_Transacciones_Exchanges FOREIGN KEY (ExchangeID) REFERENCES Exchanges(ExchangeID)
);
