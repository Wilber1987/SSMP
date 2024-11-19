CREATE PROCEDURE RenameTablesWithPrefix
    @oldPrefix NVARCHAR(50),
    @newPrefix NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql NVARCHAR(MAX);
    DECLARE @schemaName NVARCHAR(128);
    DECLARE @tableName NVARCHAR(128);
    DECLARE @newTableName NVARCHAR(128);

    -- Inicializar la variable @sql
    SET @sql = '';

    -- Cursor para recorrer todas las tablas que empiezan con @oldPrefix
    DECLARE table_cursor CURSOR FOR
    SELECT TABLE_SCHEMA, TABLE_NAME
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_TYPE = 'BASE TABLE'
      AND TABLE_NAME LIKE @oldPrefix + '%';

    -- Abrir el cursor
    OPEN table_cursor;

    -- Obtener la primera tabla
    FETCH NEXT FROM table_cursor INTO @schemaName, @tableName;

    -- Recorrer todas las tablas
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Generar el nuevo nombre de la tabla
        SET @newTableName = @newPrefix + SUBSTRING(@tableName, LEN(@oldPrefix) + 1, LEN(@tableName) - LEN(@oldPrefix));
        
        -- Construir la instrucción sp_rename
        SET @sql = 'EXEC sp_rename ''' + @schemaName + '.' + @tableName + ''', ''' + @newTableName + ''';';

        -- Ejecutar la instrucción sp_rename
        EXEC sp_executesql @sql;

        -- Obtener la siguiente tabla
        FETCH NEXT FROM table_cursor INTO @schemaName, @tableName;
    END

    -- Cerrar y liberar el cursor
    CLOSE table_cursor;
    DEALLOCATE table_cursor;
END
GO

EXEC  RenameTablesWithPrefix 'Casetable_', 'Tbl_'
