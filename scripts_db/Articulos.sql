use CURSO

drop table Libros 
create table Libros 
(
  ID                    uniqueidentifier     not null default(newid()),
  Clave_Origen          varchar(50),
  ISBN13                varchar(20),
  ISBN10                varchar(20),
  Titulo                varchar(150),
  Subtitulo             varchar(250),
  Fecha_Publicacion     date,
  Paginas               int,
  --  Editorial             varchar(100),
  ID_Editorial          uniqueidentifier,
  Tipo_Publico          int,
  --  Tipo_Publico          varchar(20),
  Descripcion           varchar(max),
  Categoria             varchar(150),
  Precio                numeric(10, 4),
  Moneda                varchar(10),
  Promedio_Rating       float,
  Comentarios           int,
  Idioma                varchar(5),
  LinkCanonico          varchar(400),
  LinkImagen            varchar(400),
  LinkInfo              varchar(400),
  --
  constraint PK_Libros primary key (ID)
)

select * from Libros

delete from Libros

create index IX_Libros_ISBN on Libros(ISBN13)


create table Autores 
(
  ID              int           identity      not null,
  Nombre          varchar(150)  not null,
  Bio             varchar(max),
  --
  constraint PK_Autores primary key (ID)
)


drop table Libros_Autores

create table Libros_Autores
(
  ID_Libro        uniqueidentifier    not null,
  ID_Autor        int                 not null,
  --
  constraint PK_Libros_Autores primary key (ID_Libro, ID_Autor),
  constraint FK_Libros_Autores_Libros foreign key (ID_Libro) references Libros(ID),
  constraint FK_Libros_Autores_Autores foreign key (ID_Autor) references Autores(ID)
)

