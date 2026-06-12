-- Tabla de notificaciones in-app (additiva, propia de la app nueva; no modifica el esquema legacy).
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'NOTIFICACION')
BEGIN
    CREATE TABLE dbo.NOTIFICACION
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_NOTIFICACION PRIMARY KEY,
        CodProveedor  VARCHAR(9)   NOT NULL,
        Tipo          VARCHAR(30)  NOT NULL,
        Titulo        VARCHAR(150) NOT NULL,
        Mensaje       VARCHAR(500) NULL,
        IdEnvio       INT          NULL,
        FechaCreacion DATETIME     NOT NULL CONSTRAINT DF_NOTIFICACION_Fecha DEFAULT (GETDATE()),
        Leida         BIT          NOT NULL CONSTRAINT DF_NOTIFICACION_Leida DEFAULT (0),
        FechaLeida    DATETIME     NULL
    );

    CREATE INDEX IX_NOTIFICACION_Proveedor_Leida ON dbo.NOTIFICACION (CodProveedor, Leida);
END
