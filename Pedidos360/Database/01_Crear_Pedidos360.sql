/*
Script de creacion de la base de datos

Si la DB ya existe, este script la ELIMINA y la vuelve a crear vacia.
*/

USE master;
GO

IF DB_ID(N'Pedidos360') IS NOT NULL
BEGIN
    ALTER DATABASE Pedidos360 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Pedidos360;
END
GO

CREATE DATABASE Pedidos360;
GO

USE Pedidos360;
GO


-- Seguridad: usuarios y roles (Identity)

CREATE TABLE dbo.Roles (
    Id                  NVARCHAR(450)   NOT NULL,
    Nombre              NVARCHAR(256)   NULL,
    NormalizedName      NVARCHAR(256)   NULL,
    ConcurrencyStamp    NVARCHAR(MAX)   NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);
GO

CREATE TABLE dbo.Usuarios (
    Id                      NVARCHAR(450)   NOT NULL,
    NombreUsuario           NVARCHAR(256)   NULL,
    NormalizedUserName      NVARCHAR(256)   NULL,
    Correo                  NVARCHAR(256)   NULL,
    NormalizedEmail         NVARCHAR(256)   NULL,
    EmailConfirmed          BIT             NOT NULL,
    HashContrasenna         NVARCHAR(MAX)   NULL,
    SecurityStamp           NVARCHAR(MAX)   NULL,
    ConcurrencyStamp        NVARCHAR(MAX)   NULL,
    PhoneNumber             NVARCHAR(MAX)   NULL,
    PhoneNumberConfirmed    BIT             NOT NULL,
    TwoFactorEnabled        BIT             NOT NULL,
    LockoutEnd              DATETIMEOFFSET  NULL,
    LockoutEnabled          BIT             NOT NULL,
    AccessFailedCount       INT             NOT NULL,
    CONSTRAINT PK_Usuarios PRIMARY KEY (Id)
);
GO

-- Relacion N:M entre usuarios y roles (Admin, Ventas, Operaciones)
CREATE TABLE dbo.UsuarioRoles (
    IdUsuario   NVARCHAR(450)   NOT NULL,
    IdRol       NVARCHAR(450)   NOT NULL,
    CONSTRAINT PK_UsuarioRoles PRIMARY KEY (IdUsuario, IdRol),
    CONSTRAINT FK_UsuarioRoles_Usuarios_IdUsuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UsuarioRoles_Roles_IdRol FOREIGN KEY (IdRol) REFERENCES dbo.Roles (Id) ON DELETE CASCADE
);
GO

-- Tablas internas de Identity (permisos, inicios de sesion externos y tokens)
CREATE TABLE dbo.RolClaims (
    Id          INT IDENTITY(1,1)   NOT NULL,
    IdRol       NVARCHAR(450)       NOT NULL,
    ClaimType   NVARCHAR(MAX)       NULL,
    ClaimValue  NVARCHAR(MAX)       NULL,
    CONSTRAINT PK_RolClaims PRIMARY KEY (Id),
    CONSTRAINT FK_RolClaims_Roles_IdRol FOREIGN KEY (IdRol) REFERENCES dbo.Roles (Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.UsuarioClaims (
    Id          INT IDENTITY(1,1)   NOT NULL,
    IdUsuario   NVARCHAR(450)       NOT NULL,
    ClaimType   NVARCHAR(MAX)       NULL,
    ClaimValue  NVARCHAR(MAX)       NULL,
    CONSTRAINT PK_UsuarioClaims PRIMARY KEY (Id),
    CONSTRAINT FK_UsuarioClaims_Usuarios_IdUsuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.UsuarioLogins (
    LoginProvider       NVARCHAR(128)   NOT NULL,
    ProviderKey         NVARCHAR(128)   NOT NULL,
    ProviderDisplayName NVARCHAR(MAX)   NULL,
    IdUsuario           NVARCHAR(450)   NOT NULL,
    CONSTRAINT PK_UsuarioLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_UsuarioLogins_Usuarios_IdUsuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (Id) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.UsuarioTokens (
    IdUsuario       NVARCHAR(450)   NOT NULL,
    LoginProvider   NVARCHAR(128)   NOT NULL,
    Name            NVARCHAR(128)   NOT NULL,
    Value           NVARCHAR(MAX)   NULL,
    CONSTRAINT PK_UsuarioTokens PRIMARY KEY (IdUsuario, LoginProvider, Name),
    CONSTRAINT FK_UsuarioTokens_Usuarios_IdUsuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (Id) ON DELETE CASCADE
);
GO


-- Catalogo


CREATE TABLE dbo.Categorias (
    Id      INT IDENTITY(1,1)   NOT NULL,
    Nombre  NVARCHAR(50)        NOT NULL,
    CONSTRAINT PK_Categorias PRIMARY KEY (Id)
);
GO

-- La imagen se guarda en la base como VARBINARY, no como enlace.
-- ImpuestoPorc depende de cada producto.
CREATE TABLE dbo.Productos (
    Id              INT IDENTITY(1,1)   NOT NULL,
    Codigo          NVARCHAR(20)        NOT NULL,
    Nombre          NVARCHAR(100)       NOT NULL,
    CategoriaId     INT                 NOT NULL,
    Precio          DECIMAL(18,2)       NOT NULL,
    ImpuestoPorc    DECIMAL(5,2)        NOT NULL,
    Stock           INT                 NOT NULL,
    Imagen          VARBINARY(MAX)      NOT NULL,
    ImagenTipo      NVARCHAR(30)        NOT NULL,
    Activo          BIT                 NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1),
    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),
    CONSTRAINT CK_Productos_ImpuestoPorc CHECK (ImpuestoPorc BETWEEN 0 AND 100),
    CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0),
    CONSTRAINT FK_Productos_Categorias_CategoriaId FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias (Id) ON DELETE NO ACTION
);
GO


-- Clientes


-- TipoIdentificacion: 1 = Fisica (9 digitos), 2 = Juridica (10 digitos)
CREATE TABLE dbo.Clientes (
    Id                  INT IDENTITY(1,1)   NOT NULL,
    TipoIdentificacion  INT                 NOT NULL,
    Cedula              NVARCHAR(10)        NOT NULL,
    Nombre              NVARCHAR(150)       NOT NULL,
    Correo              NVARCHAR(100)       NOT NULL,
    Telefono            NVARCHAR(8)         NOT NULL,
    Direccion           NVARCHAR(250)       NOT NULL,
    CONSTRAINT PK_Clientes PRIMARY KEY (Id),
    CONSTRAINT CK_Clientes_TipoIdentificacion CHECK (TipoIdentificacion IN (1, 2))
);
GO


-- Pedidos


-- Estado: 1 = Confirmado, 2 = Anulado. El pedido se guarda al confirmarse.
-- Subtotal, Impuestos y Total se persisten para auditoria.
CREATE TABLE dbo.Pedidos (
    Id          INT IDENTITY(1,1)   NOT NULL,
    ClienteId   INT                 NOT NULL,
    UsuarioId   NVARCHAR(450)       NOT NULL,
    Fecha       DATETIME2           NOT NULL,
    Estado      INT                 NOT NULL,
    Subtotal    DECIMAL(18,2)       NOT NULL,
    Impuestos   DECIMAL(18,2)       NOT NULL,
    Total       DECIMAL(18,2)       NOT NULL,
    CONSTRAINT PK_Pedidos PRIMARY KEY (Id),
    CONSTRAINT CK_Pedidos_Estado CHECK (Estado IN (1, 2)),
    CONSTRAINT CK_Pedidos_Subtotal CHECK (Subtotal >= 0),
    CONSTRAINT CK_Pedidos_Impuestos CHECK (Impuestos >= 0),
    CONSTRAINT CK_Pedidos_Total CHECK (Total = Subtotal + Impuestos),
    CONSTRAINT FK_Pedidos_Clientes_ClienteId FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes (Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Pedidos_Usuarios_UsuarioId FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios (Id) ON DELETE NO ACTION
);
GO

-- PrecioUnit e ImpuestoPorc son los valores historicos al vender.
-- TipoDescuento: 1 = Porcentaje, 2 = Monto fijo. TotalLinea se persiste para auditoria.
CREATE TABLE dbo.PedidoDetalles (
    Id              INT IDENTITY(1,1)   NOT NULL,
    PedidoId        INT                 NOT NULL,
    ProductoId      INT                 NOT NULL,
    Cantidad        INT                 NOT NULL,
    PrecioUnit      DECIMAL(18,2)       NOT NULL,
    TipoDescuento   INT                 NOT NULL,
    Descuento       DECIMAL(18,2)       NOT NULL,
    ImpuestoPorc    DECIMAL(5,2)        NOT NULL,
    TotalLinea      DECIMAL(18,2)       NOT NULL,
    CONSTRAINT PK_PedidoDetalles PRIMARY KEY (Id),
    CONSTRAINT CK_PedidoDetalles_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_PedidoDetalles_PrecioUnit CHECK (PrecioUnit > 0),
    CONSTRAINT CK_PedidoDetalles_TipoDescuento CHECK (TipoDescuento IN (1, 2)),
    CONSTRAINT CK_PedidoDetalles_Descuento CHECK (
        Descuento >= 0 AND (
            (TipoDescuento = 1 AND Descuento <= 100) OR
            (TipoDescuento = 2 AND Descuento <= Cantidad * PrecioUnit))),
    CONSTRAINT CK_PedidoDetalles_ImpuestoPorc CHECK (ImpuestoPorc BETWEEN 0 AND 100),
    CONSTRAINT CK_PedidoDetalles_TotalLinea CHECK (TotalLinea >= 0),
    CONSTRAINT FK_PedidoDetalles_Pedidos_PedidoId FOREIGN KEY (PedidoId) REFERENCES dbo.Pedidos (Id) ON DELETE CASCADE,
    CONSTRAINT FK_PedidoDetalles_Productos_ProductoId FOREIGN KEY (ProductoId) REFERENCES dbo.Productos (Id) ON DELETE NO ACTION
);
GO


-- Indices

-- Seguridad (Identity)
CREATE UNIQUE INDEX RoleNameIndex ON dbo.Roles (NormalizedName) WHERE NormalizedName IS NOT NULL;
CREATE UNIQUE INDEX UserNameIndex ON dbo.Usuarios (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
CREATE INDEX EmailIndex ON dbo.Usuarios (NormalizedEmail);
CREATE INDEX IX_UsuarioRoles_IdRol ON dbo.UsuarioRoles (IdRol);
CREATE INDEX IX_RolClaims_IdRol ON dbo.RolClaims (IdRol);
CREATE INDEX IX_UsuarioClaims_IdUsuario ON dbo.UsuarioClaims (IdUsuario);
CREATE INDEX IX_UsuarioLogins_IdUsuario ON dbo.UsuarioLogins (IdUsuario);

-- Unicidad de negocio
CREATE UNIQUE INDEX IX_Categorias_Nombre ON dbo.Categorias (Nombre);
CREATE UNIQUE INDEX IX_Productos_Codigo ON dbo.Productos (Codigo);
CREATE UNIQUE INDEX IX_Clientes_Cedula ON dbo.Clientes (Cedula);
CREATE UNIQUE INDEX IX_PedidoDetalles_PedidoId_ProductoId ON dbo.PedidoDetalles (PedidoId, ProductoId);

-- Búsquedas y llaves foraneas
CREATE INDEX IX_Productos_Nombre ON dbo.Productos (Nombre);
CREATE INDEX IX_Productos_CategoriaId ON dbo.Productos (CategoriaId);
CREATE INDEX IX_Clientes_Nombre ON dbo.Clientes (Nombre);
CREATE INDEX IX_Pedidos_ClienteId ON dbo.Pedidos (ClienteId);
CREATE INDEX IX_Pedidos_UsuarioId ON dbo.Pedidos (UsuarioId);
CREATE INDEX IX_PedidoDetalles_ProductoId ON dbo.PedidoDetalles (ProductoId);
GO

PRINT N'Base de datos Pedidos360 creada correctamente.';
GO
