# Fase 3 — Mapeo de entidades de dominio ↔ esquema legacy

Las entidades de dominio (`src/ControlEnvios.Domain/Entities`) usan nomenclatura limpia y tipos
modernos. Como aún **no hay acceso a la BD `Bascula`**, se han definido manualmente a partir de los
modelos EF6 del legacy. Este documento registra el mapeo **propiedad → columna** para configurar la
**Fluent API de EF Core** (o validar el scaffold) en Infraestructura cuando se disponga de la BD.

> Convención: las claves/columnas se mapearán con `ToTable("...")` + `HasColumnName("...")` en
> `IEntityTypeConfiguration<T>` dentro de `ControlEnvios.Infrastructure/Persistence/Configurations`.

## Proveedor → `PROVEEDOR`  (PK `CODPROVEEDOR`)
| Dominio | Columna | Tipo |
|---------|---------|------|
| Codigo (PK) | CODPROVEEDOR | string |
| Nombre | NOMBRE | string? |
| Cif | CIF | string? |
| Direccion | DIRECCION | string? |
| Poblacion | POBLACION | string? |
| Provincia | PROVINCIA | string? |
| CodigoPostal | CODPOSTAL | string? |
| Observaciones | OBSERVACIONES | string? |
| Email | EMAIL | string? |
| Password | PASSWORD | string? → hash en Fase 6 |

## Envio → `ENVIOMERCANCIA`  (PK `IDENVIO`, identity)
| Dominio | Columna | Tipo |
|---------|---------|------|
| Id (PK) | IDENVIO | int |
| CodigoProveedor | CODPROVEEDOR | string |
| CodigoArticulo | CODARTICULO | string |
| FechaEnvio | FECHAENVIO | DateOnly ← date |
| HoraEnvio | HORAENVIO | TimeOnly? ← datetime (solo hora) |
| KilosEnviados | KILOSENVIADOS | decimal |
| Estado | ESTADO | int → enum EstadoEnvio |
| Matricula | MATRICULA | string? |
| Observaciones | OBSERVACIONES | string? |
| EnviarEmail | SENDEMAIL | bool |
| TieneNeto | TIENENETO | int *(confirmar semántica)* |

## Articulo → `ARTICULO`  (PK `ID_ARTICULO`)
| Dominio | Columna |
|---------|---------|
| Id (PK) | ID_ARTICULO |
| Codigo | CODARTICULO |
| Descripcion | DESCRIPCION |
| ActivoWeb | ACTIVAWEB (bool?) |

## CupoProveedor → `CUPOPROVEEDOR`  (PK `IDCUPOPROV`)
| Dominio | Columna |
|---------|---------|
| Id (PK) | IDCUPOPROV |
| CodigoProveedor | CODPROVEE |
| CodigoArticulo | CODARTICULO |
| FechaInicio | FECHAINICIOCUPO |
| FechaFin | **FEHAFINALCUPO** *(typo en el esquema legacy)* |
| PorcentajeAsignado | PORCENTAGEASIG (decimal?) |
| Activo | ACTIVACUPO (bool) |

## CupoGlobal → `CUPOKG`  (PK `IDCUPOKG`)
| Dominio | Columna |
|---------|---------|
| Id (PK) | IDCUPOKG |
| CupoKilos | CUPOKILOS |
> El legacy lee siempre la fila `IDCUPOKG == 1`.

## Usuario → `USUARIOS`  (PK `ID_USUARIO`)
| Dominio | Columna |
|---------|---------|
| Id (PK) | ID_USUARIO |
| Perfil | ID_PERFIL → enum PerfilUsuario |
| Nombre | NOMBRE |
| Alias | ALIAS |
| Password | PASSWORD → hash en Fase 6 |
| AccesoWebAdmin | CHECKWEBADMIN |

## Modulo → `MODULO`  (PK `IdModulo`)
| Dominio | Columna |
|---------|---------|
| Id (PK) | IdModulo |
| Nombre | NombreModulo |
| Accion | **Acction** *(typo legacy)* |
| Controlador | Controller |
| Icono | icono |
| FondoNombre | bgName |

## EstadoMercancia → `ESTADOMERCANCIA`  (PK `ID`)
| Dominio | Columna |
|---------|---------|
| Id (PK) | ID |
| Nombre | ESTADOMERCANCIA1 |

## PesadaBascula → `PESADABASCULA`  (PK `ID_PESADA`)
| Dominio | Columna | | Dominio | Columna |
|---------|---------|-|---------|---------|
| Id (PK) | ID_PESADA | | InfoModificacionPesada | INFOMODPESADA |
| IdTicket | ID_TICKET | | EstadoCamion | ESTADOCAMION |
| Matricula | MATRICULA | | EstadoMercancia | ESTADOMERCANCIA |
| Remolque | REMOLQUE | | BasculaGrande | BALSAGRANDE |
| CodigoProveedor | COD_PROV | | BasculaPequena | BALSAPEQUENA |
| CodigoCliente | COD_CLIE | | IdEnvioWeb | IDENVIOWEB |
| CodigoArticulo | COD_ART | | EmailEnviado | EMAILENVIADO |
| PrecioKg | PRECIOKG | | VgmTara | VGMTARA |
| FechaEntrada | FECHAENTRADA | | Pesada1 | PESADA1 |
| HoraEntrada | HORAENTRADA | | Pesada2 | PESADA2 |
| FechaSalida | FECHASALIDA | | PesadaNeto | PESADANETO |
| HoraSalida | HORASALIDA | | PrimeraPesada | PRIMEPESADA |
| Observaciones | OBSERVACIONES | | SegundaPesada | SECUNPESADA |
| UsuarioPesada | USERPESADA | | | |

---

## Procedimientos almacenados (pendientes de scaffold real en Fase 3)
No se modelan como entidades. Se invocarán con `FromSql`/`SqlQuery` y DTOs de resultado en la capa
de Infraestructura/Aplicación. Contrato documentado en
[`FASE1-CARACTERIZACION.md`](FASE1-CARACTERIZACION.md#2-catálogo-de-procedimientos-almacenados):
`ENVIOPESADA`, `LISTAENVIOSIDPROVEEDOR`, `LISTAENVIOSFECHAPROVEEDOR`, `KILOS_SEMANA_PROVEEDOR`,
`CANCELAENVIO`, `CANCELAENVIOEMAIL`, `TODOSLOSENVIOSPROVEE`, `ALLCUPOSPROVEEDOR`,
`RECUPERAENVIOSPROVEE`/`RECUPERAENVIOSPROVEE1`.

## Validación contra la BD real ✅ (servidor `Bascula`)
Verificado con `RealDbSmokeTests` (consulta de las 9 tablas) y por inspección de
`INFORMATION_SCHEMA`. El esquema coincide con las entidades, salvo los ajustes siguientes ya aplicados:

- **`ESTADOMERCANCIA`**: la columna se llama `ESTADOMERCANCIA` (no `ESTADOMERCANCIA1`, que era un
  artefacto del EDMX legacy). Corregido en `EstadoMercanciaConfiguration`.
- **Fechas `datetime`**: `FECHAENVIO`, `HORAENVIO`, `FECHAINICIOCUPO`, `FEHAFINALCUPO` son `datetime`
  en la BD. Se aplican conversores (`DateOnlyToDateTimeConverter`, `TimeOnlyToDateTimeConverter`) para
  conservar `DateOnly`/`TimeOnly` en el dominio sin tocar el esquema.
- **`PESADABASCULA`** tiene una columna extra `BALSA3 (bit)` no usada por la web: EF la ignora (solo
  selecciona las propiedades mapeadas). Sin acción.
- Longitudes reales relevantes: `PROVEEDOR.PASSWORD varchar(100)`, **`USUARIOS.PASSWORD varchar(50)`**.

> ⚠️ **Ancho de columnas para el hashing (Fase 7).** Un hash PBKDF2 ocupa ~83 caracteres.
> `USUARIOS.PASSWORD varchar(50)` es **insuficiente**. Antes de activar el re-hash de contraseñas
> (`Auth:RehashPasswordsOnLogin`) en el corte, hay que **ampliar** `USUARIOS.PASSWORD` (y por margen
> `PROVEEDOR.PASSWORD`) a `varchar(200)`. Mientras la BD se comparta con el sistema legacy de báscula,
> el re-hash permanece **desactivado** (rompería el login del legacy, que compara texto plano).

## Pendiente (con BD)
- Mapear los procedimientos almacenados restantes (Gestor/Producción: `LISTAENVIOSFECHAPROVEEDOR`, etc.).
- Confirmar el catálogo completo de `ESTADOMERCANCIA` y la semántica de `TIENENETO`.
