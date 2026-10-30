# supermercado

Instalar paquete para conectarnos a mariadb
dotnet add package MySqlConnector


create database supermercado;

create table producto(
    id_producto int not null auto_increment primary key,
    nombre varchar(20),
    precio int
);