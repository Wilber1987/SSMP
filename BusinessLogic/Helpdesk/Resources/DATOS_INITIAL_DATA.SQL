delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Comments_Tasks 
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Comments
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Mails  
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Calendario 
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Participantes  
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Tareas 
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Profile_CasosAsignados  
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Case


delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Agenda 
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Servicios_Profile 
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Servicios
delete from PROYECT_MANAGER_BD.helpdesk.Tbl_Dependencias_Usuarios 
delete from PROYECT_MANAGER_BD.helpdesk.Cat_Dependencias 

DBCC CHECKIDENT ('PROYECT_MANAGER_BD.helpdesk.Cat_Dependencias', RESEED, 0);
DBCC CHECKIDENT ('PROYECT_MANAGER_BD.helpdesk.Tbl_Servicios', RESEED, 0);


--DATOS BASICOS DE PRUEBA
INSERT INTO PROYECT_MANAGER_BD.helpdesk.Cat_Dependencias
(Descripcion, Id_Dependencia_Padre, Username, Password, Host, AutenticationType, TENAT, CLIENT, OBJECTID, CLIENT_SECRET, HostService, SMTPHOST, DefaultDependency)
VALUES(N'SSMP', NULL,NULL,NULL,NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);


INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Servicios
(Nombre_Servicio, Descripcion_Servicio, Id_Tipo_Servicio, Visibilidad, Estado, Fecha_Inicio, Fecha_Finalizacion, Id_Dependencia)
VALUES(N'Servicio de seguimiento', N'seguimiento', 1, N'publica', N'Activo', '2021-01-12', '2021-12-12', 1);

--Agenda dependencia


INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Agenda
(Id_Perfil, Id_Dependencia, Dia, Hora_Inicial, Hora_Final, Fecha_Caducidad)
VALUES(NULL, 1, N'Lunes', N'08:00', N'17:00', '2024-06-07 00:00:00.000');
INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Agenda
(Id_Perfil, Id_Dependencia, Dia, Hora_Inicial, Hora_Final, Fecha_Caducidad)
VALUES(NULL, 1, N'Martes', N'08:00', N'17:00', '2024-06-07 00:00:00.000');
INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Agenda
(Id_Perfil, Id_Dependencia, Dia, Hora_Inicial, Hora_Final, Fecha_Caducidad)
VALUES(NULL, 1, N'Miércoles', N'08:00', N'17:00', '2024-06-07 00:00:00.000');
INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Agenda
(Id_Perfil, Id_Dependencia, Dia, Hora_Inicial, Hora_Final, Fecha_Caducidad)
VALUES(NULL, 1, N'Jueves', N'08:00', N'17:00', '2024-06-07 00:00:00.000');
INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Agenda
(Id_Perfil, Id_Dependencia, Dia, Hora_Inicial, Hora_Final, Fecha_Caducidad)
VALUES(NULL, 1, N'Viernes', N'08:00', N'17:00', '2024-06-07 00:00:00.000');

--seguridad

DELETE FROM PROYECT_MANAGER_BD.[security].Security_Users_Roles;
DELETE FROM PROYECT_MANAGER_BD.[security].Security_Permissions_Roles;
DELETE FROM PROYECT_MANAGER_BD.[security].Tbl_Profile;
DELETE FROM PROYECT_MANAGER_BD.[security].Security_Users;
DELETE FROM PROYECT_MANAGER_BD.[security].Security_Permissions;
DELETE FROM PROYECT_MANAGER_BD.[security].Security_Roles;

DBCC CHECKIDENT ('PROYECT_MANAGER_BD.[security].Security_Users', RESEED, 0);
DBCC CHECKIDENT ('PROYECT_MANAGER_BD.[security].Security_Permissions', RESEED, 0);
DBCC CHECKIDENT ('PROYECT_MANAGER_BD.[security].Security_Roles', RESEED, 0);
DBCC CHECKIDENT ('PROYECT_MANAGER_BD.[security].Tbl_Profile', RESEED, 0);


-- ALTER TABLE PROYECT_MANAGER_BD.[security].Security_Permissions ADD Detalles nvarchar(500) NULL;


INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'ADMIN_ACCESS', N'Activo', N'ACCESO TOTAL DEL SISTEMA');
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'ADMINISTRAR_CASOS_DEPENDENCIA', N'Activo', N'PUEDE VER LOS CASOS DE TODA LA DEPENDENCIA A LA QUE PERTENECE, APROBARLOS, RECHAZARLOS, CERRARLOS');
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'TECNICO_CASOS_DEPENDENCIA', N'Activo', N'PUEDE VER LOS CASOS ASIGNADOS Y CERRARLOS');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'GENERADOR_SOLICITUDES', N'Activo', N'PERMITE REALIZAR SOLICITUDES DE CASOS DENTRO DE LA PLATAFORMA');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'ADMINISTRAR_CASOS_PROPIOS', N'Activo', N'PUEDE VER LOS CASOS PROPIOS');
--PERFILES
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'PERFIL_ACCESS', N'Activo', N'PERMITE ACCEDER AL PERFIL DEL USUARIO');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'PERFIL_MANAGER', N'Activo', N'PERMITE MANIPULAR PERFILES DE LOS USUARIO');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'ADMINISTRAR_USUARIOS', N'Activo', N'PERMITE MANIPULAR PERFILES DE TODOS LOS USUARIO');



-- mensajeria
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'SEND_MESSAGE', N'Activo', N'PUEDE ENVIAR MENSAJES A LOS USUARIOS');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'NOTIFICACIONES_READER', N'Activo', N'PUEDE LEER NOTIFICACIONES');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'NOTIFICACIONES_MANAGER', N'Activo', N'PUEDE CREAR NOTIFICACIONES');


-- questionaries
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'QUESTIONNAIRES_MANAGER', N'Activo', N'PUEDE CREAR QUESTIONARIOS');
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'QUESTIONNAIRES_GESTOR', N'Activo', N'PUEDE GESTIONAR QUESTIONARIOS');
INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
(Descripcion, Estado, Detalles)
VALUES(N'QUESTIONNAIRES_USER', N'Activo', N'PUEDE RESOLVER QUESTIONARIOS');


INSERT INTO PROYECT_MANAGER_BD.[security].Security_Roles
(Descripcion, Estado)
VALUES(N'ADMIN', N'ACTIVO');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions_Roles 
VALUES(1, 1, N'ACTIVO');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Users
(Nombres, Estado, Descripcion, Password, Mail, Token, Token_Date, Token_Expiration_Date)
VALUES(N'ADMIN', N'ACTIVO', N'ADMIN', N'ixKQx9yhnazTxAdS4GGV2g==', N'admin@admin.net', NULL, NULL, NULL);

INSERT INTO PROYECT_MANAGER_BD.[security].Tbl_Profile
(Nombres, Apellidos, FechaNac, IdUser, Sexo, Foto, DNI, Correo_institucional, Id_Pais_Origen, Estado)
VALUES(N'ADMIN', N'ADMIN', '1900-01-01', 1, N'Masculino', N'\Media\profiles\avatar.png', N'1112dfg', N'admin@admin.net', NULL, N'ACTIVO');

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Users_Roles
(Id_Role, Id_User, Estado)
VALUES(1, 1, N'ACTIVO');

INSERT INTO PROYECT_MANAGER_BD.helpdesk.Tbl_Dependencias_Usuarios
(Id_Perfil, Id_Dependencia, Id_Cargo)
VALUES(1, 1, NULL);



