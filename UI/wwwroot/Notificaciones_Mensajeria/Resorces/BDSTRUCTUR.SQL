-- DROP SCHEMA notificaciones_mensajeria;

CREATE SCHEMA notificaciones_mensajeria;
-- PROYECT_MANAGER_BD.notificaciones_mensajeria.conversacion definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.notificaciones_mensajeria.conversacion;

CREATE TABLE conversacion (
	id_conversacion int IDENTITY(1,1) NOT NULL,
	descripcion nvarchar(255) COLLATE Latin1_General_CI_AI NULL,
	CONSTRAINT PK__conversa__DA6816878A0519D6 PRIMARY KEY (id_conversacion)
);


-- PROYECT_MANAGER_BD.notificaciones_mensajeria.notificaciones definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.notificaciones_mensajeria.notificaciones;

CREATE TABLE notificaciones (
	Id int IDENTITY(1,1) NOT NULL,
	Id_User int NULL,
	Titulo nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL,
	Mensaje nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL,
	Fecha datetime NULL,
	Media nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL,
	Tipo nvarchar(50) COLLATE Latin1_General_CI_AI NULL,
	Estado nvarchar(50) COLLATE Latin1_General_CI_AI NULL,
	Enviado bit NULL,
	Leido bit NULL,
	Telefono nvarchar(20) COLLATE Latin1_General_CI_AI NULL,
	Email nvarchar(50) COLLATE Latin1_General_CI_AI NULL,
	CONSTRAINT PK_notificaciones PRIMARY KEY (Id)
);


-- PROYECT_MANAGER_BD.notificaciones_mensajeria.conversacion_usuarios definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.notificaciones_mensajeria.conversacion_usuarios;

CREATE TABLE conversacion_usuarios (
	id_conversacion int NOT NULL,
	id_usuario int NOT NULL,
	avatar nvarchar(250) COLLATE Latin1_General_CI_AI NULL,
	name nvarchar(250) COLLATE Latin1_General_CI_AI NULL,
	CONSTRAINT conversacion_usuarios_PK PRIMARY KEY (id_conversacion,id_usuario),
	CONSTRAINT conversacion_usuarios_FK FOREIGN KEY (id_conversacion) REFERENCES conversacion(id_conversacion)
);


-- PROYECT_MANAGER_BD.notificaciones_mensajeria.mensajes definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.notificaciones_mensajeria.mensajes;

CREATE TABLE mensajes (
	id_mensaje int IDENTITY(1,1) NOT NULL,
	remitente nvarchar(255) COLLATE Latin1_General_CI_AI NULL,
	usuario_id int NULL,
	destinatarios nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL,
	asunto nvarchar(255) COLLATE Latin1_General_CI_AI NULL,
	body nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL,
	id_conversacion int NULL,
	created_at datetime NOT NULL,
	updated_at datetime NOT NULL,
	enviado bit NULL,
	leido bit NULL,
	CONSTRAINT PK__mensajes__5B37C7F609C6AFC5 PRIMARY KEY (id_mensaje),
	CONSTRAINT FK_mensajes_conversacion FOREIGN KEY (id_conversacion) REFERENCES conversacion(id_conversacion)
);


-- PROYECT_MANAGER_BD.notificaciones_mensajeria.mensaje_adjuntos definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.notificaciones_mensajeria.mensaje_adjuntos;

CREATE TABLE mensaje_adjuntos (
	id int NOT NULL,
	mensaje_id int NULL,
	archivo nvarchar(255) COLLATE Latin1_General_CI_AI NULL,
	created_at datetime NOT NULL,
	updated_at datetime NOT NULL,
	CONSTRAINT PK__mensaje___3213E83FC56F1A7B PRIMARY KEY (id),
	CONSTRAINT FK_mensaje_adjuntos_mensajes FOREIGN KEY (mensaje_id) REFERENCES mensajes(id_mensaje)
);
CREATE NONCLUSTERED INDEX index_mensaje_adjuntos_on_mensaje_id ON PROYECT_MANAGER_BD.notificaciones_mensajeria.mensaje_adjuntos (mensaje_id);