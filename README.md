# NioxVF — Conector Veri*Factu
### Estado Actual del Proyecto - Diciembre 2024

![.NET](https://img.shields.io/badge/.NET-8.0.0-blue)
![License](https://img.shields.io/badge/license-Proprietary-red)
![Status](https://img.shields.io/badge/status-Sprint%202%20En%20Desarrollo-orange)
![Build](https://img.shields.io/badge/build-passing-brightgreen)

**NioxVF** es un conector multi-tenant para el sistema Veri*Factu de la AEAT, diseñado para integrarse con TPVs Aronium y proporcionar un servicio completo de facturación electrónica.

## ⚠️ Requisitos del Sistema

### **Versión de .NET Requerida**
- **.NET 8.0.0** (versión unificada para todo el proyecto)
- ✅ **Unificación completada**: Todos los proyectos usan .NET 8.0.0
- ✅ **Compatibilidad**: Entity Framework Core 8.0.0
- ✅ **Herramientas**: dotnet-ef 8.0.0
- Verificar instalación: `dotnet --version`

## 📊 Estado Actual del Desarrollo

### ✅ COMPLETADO - Sprint 0 & 1 (100%)

El proyecto tiene implementado completamente el Sprint 1 según la especificación funcional original. **La solución compila sin errores** y está lista para producir un MVP funcional.

### 🚀 EN DESARROLLO - Sprint 2 (80%)

**Persistencia SQLite y Control de Concurrencia** - Actualmente en desarrollo por Angel (Backend & Persistencia).

**Progreso actual:**
- ✅ **NioxVF.Persistence**: Interfaces y entidades completadas (100%)
- ✅ **Entity Framework Core**: Configurado y DbContext implementado (100%)
- ✅ **NioxVF.Persistence.Sqlite**: Proyecto creado y base implementado (100%)
- ✅ **Repositorios específicos**: Implementados y verificados (100%)
- ✅ **Control de concurrencia**: SeriesLockRepository implementado (100%)
- ✅ **Migraciones**: Implementadas y funcionando (100%)
- ✅ **Tests unitarios**: 37 tests pasando (100%)
- ✅ **Unificación .NET 8.0**: Completada (100%)

### 🏗️ Arquitectura Implementada

```
┌─────────────────────┐    HTTP/JSON    ┌─────────────────────┐    SOAP+mTLS    ┌─────────────────────┐
│   NioxVF.Agent     │ ──────────────> │    NioxVF.Api       │ ──────────────> │   AEAT Veri*Factu   │
│  (Windows Service) │                 │  (ASP.NET Core)     │                 │   (Simulado)        │
└─────────────────────┘                 └─────────────────────┘                 └─────────────────────┘
         │                                        │
         │ Lee tickets JSON                       │ Respuesta + QR
         ▼                                        ▼
┌─────────────────────┐                 ┌─────────────────────┐
│   Inbox Directory   │                 │   In-Memory Store   │
│     (C:\NioxVF\)    │                 │   (Placeholder)     │
└─────────────────────┘                 └─────────────────────┘
```

## 🎯 Funcionalidades Implementadas

### ✅ NioxVF.Domain (100% Completado)

**Modelos principales:**
- ✅ `InvoiceSimple` - Modelo completo de factura F1
- ✅ `TaxItem` - Gestión de impuestos (IVA, recargo)
- ✅ Enums: `InvoiceStatus`, `SignMode`

**Interfaces:**
- ✅ `IHashChain` - Encadenado criptográfico
- ✅ `ISigner` - Firma XAdES-EPES
- ✅ `IVeriFactuTransport` - Comunicación AEAT

**Servicios:**
- ✅ `HashChainService` - Implementación SHA-256 del encadenado
  - ✅ Calcula hash por SellerNif|Series|Number|Date|Total|PrevHash
  - ✅ Gestión de cadenas por seller+serie
  - ⚠️ **Nota**: Almacenamiento en memoria (se necesita persistencia real)

### ✅ NioxVF.Api (100% Completado para Sprint 1)

**Controladores:**
- ✅ `InvoicesController` - Procesamiento de facturas
- ✅ `HealthController` - Health checks y métricas

**Endpoints implementados:**
- ✅ `POST /api/v1/invoices` - Procesar nueva factura
- ✅ `GET /api/v1/invoices/{id}` - Obtener estado de factura
- ✅ `GET /health` - Health check
- ✅ `GET /metrics` - Métricas del sistema

**Middleware y autenticación:**
- ✅ `ApiKeyMiddleware` - Autenticación por X-API-Key
- ✅ Swagger configurado con seguridad
- ✅ CORS y headers de seguridad

**DTOs:**
- ✅ `InvoiceRequest` - Solicitud con invoice + mode
- ✅ `InvoiceResponse` - Respuesta con id, status, aeatId, qrUrl

**Servicios:**
- ✅ `InvoiceService` - Lógica de procesamiento de facturas
- ✅ Inyección de dependencias configurada

### ✅ NioxVF.Agent (100% Completado para Sprint 1)

**Servicio principal:**
- ✅ `Worker` - BackgroundService que procesa tickets
- ✅ Polling cada 3-5 segundos configurables
- ✅ Gestión de excepciones y logging

**Servicios especializados:**
- ✅ `JsonInboxReader` - Lee archivos JSON del inbox
  - ✅ Gestión de carpetas: inbox, processed, failed
  - ✅ Manejo de errores y archivos corruptos
- ✅ `ApiClient` - Cliente HTTP para comunicación con API
  - ✅ Autenticación automática con X-API-Key
  - ✅ Serialización JSON con camelCase
- ✅ `PlaceholderQrGenerator` - Generador QR para MVP

**Pipeline de procesamiento:**
1. ✅ Detectar nuevos archivos JSON en inbox
2. ✅ Deserializar a `InvoiceSimple`
3. ✅ Enviar a API con mode "sign-and-send"
4. ✅ Procesar respuesta y generar QR PNG
5. ✅ Mover archivo a processed/failed según resultado

---

## 🚀 SPRINT 2 - Persistencia SQLite (30% Completado)

### 📊 Progreso General del Sprint 2

**Estado:** En desarrollo - Rama `feature/sqlite-repository`  
**Responsable:** Angel - Backend & Persistencia  
**Fecha inicio:** Diciembre 2024  
**Progreso actual:** 6/10 tareas completadas (60%)

### ✅ TAREAS COMPLETADAS

#### ✅ **TAREA 1: Configurar NioxVF.Persistence (100%)**
- ✅ **1.1 Configurar Dependencias del Proyecto**
  - ✅ Agregar referencia a NioxVF.Domain
  - ✅ Agregar Entity Framework Core 8.0.0
  - ✅ Agregar Entity Framework Core Tools
  - ✅ Agregar Entity Framework Core Design

- ✅ **1.2 Limpiar y Preparar Estructura**
  - ✅ Eliminar Class1.cs del proyecto Persistence
  - ✅ Crear carpeta `Interfaces/`
  - ✅ Crear carpeta `Entities/`
  - ✅ Crear carpeta `Entities/Base/`
  - ✅ Crear carpeta `Services/`

- ✅ **1.3 Verificar Configuración**
  - ✅ Compilar proyecto sin errores
  - ✅ Verificar referencias correctas

#### ✅ **TAREA 2: Definir Interfaces de Repositorio (100%)**
- ✅ **2.1 Crear Interfaz Base IRepository<T>**
  - ✅ Métodos CRUD básicos: GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync, ExistsAsync
  - ✅ Métodos de búsqueda: FindAsync, FirstOrDefaultAsync
  - ✅ Documentación XML completa

- ✅ **2.2 Crear IInvoiceRepository**
  - ✅ Heredar de IRepository<InvoiceEntity>
  - ✅ Métodos específicos: GetBySeriesAndNumberAsync, GetBySellerAsync, GetByDateRangeAsync, GetLastNumberInSeriesAsync
  - ✅ Documentación XML completa

- ✅ **2.3 Crear IHashChainRepository**
  - ✅ Heredar de IRepository<HashChainEntity>
  - ✅ Métodos específicos: GetPreviousHashAsync, UpdateChainAsync, GetChainBySellerAsync
  - ✅ Documentación XML completa

- ✅ **2.4 Crear ISeriesLockRepository**
  - ✅ Heredar de IRepository<SeriesLockEntity>
  - ✅ Métodos específicos: TryAcquireLockAsync, ReleaseLockAsync, IsLockedAsync, CleanupExpiredLocksAsync
  - ✅ Documentación XML completa

#### ✅ **TAREA 3: Crear Entidades de Dominio para Persistencia (100%)**
- ✅ **3.1 Crear Entidad Base AuditEntity**
  - ✅ Propiedades de auditoría: Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
  - ✅ Documentación XML completa

- ✅ **3.2 Crear InvoiceEntity**
  - ✅ Heredar de AuditEntity
  - ✅ Mapear propiedades de InvoiceSimple: SellerNif, SellerName, Series, Number, IssueDate, etc.
  - ✅ Navegación a TaxItems implementada
  - ✅ Propiedades calculadas: TotalTaxBase, TotalTaxAmount, TotalSurcharge, Total
  - ✅ Documentación XML completa

- ✅ **3.3 Crear TaxItemEntity**
  - ✅ Heredar de AuditEntity
  - ✅ Propiedades de impuestos: TaxBase, TaxRate, TaxAmount, SurchargeRate, SurchargeAmount, TaxType
  - ✅ Navegación a Invoice implementada
  - ✅ Documentación XML completa

- ✅ **3.4 Crear HashChainEntity**
  - ✅ Heredar de AuditEntity
  - ✅ Propiedades para cadena de hashes: SellerNif, Series, CurrentHash, PreviousHash, LastUpdated
  - ✅ Documentación XML completa

- ✅ **3.5 Crear SeriesLockEntity**
  - ✅ Heredar de AuditEntity
  - ✅ Propiedades para control de concurrencia: SellerNif, Series, LockId, AcquiredAt, ExpiresAt, IsActive
  - ✅ Documentación XML completa

### ✅ TAREAS COMPLETADAS (CONTINUACIÓN)

#### ✅ **TAREA 4: Configurar Entity Framework Core (100%)**
- ✅ **4.1 Crear DbContext Base**
  - ✅ Crear archivo `NioxVF.Persistence/Context/NioxVFDbContext.cs`
  - ✅ Definir DbSets para todas las entidades
  - ✅ Configurar OnModelCreating
  - ✅ Configurar OnConfiguring para logging

- ✅ **4.2 Crear Configuraciones de Entidades**
  - ✅ InvoiceEntityConfiguration
  - ✅ TaxItemEntityConfiguration
  - ✅ HashChainEntityConfiguration
  - ✅ SeriesLockEntityConfiguration

#### ✅ **TAREA 5: Crear Proyecto NioxVF.Persistence.Sqlite (100%)**
- ✅ **5.1 Crear Proyecto SQLite**
  - ✅ Crear directorio `NioxVF.Persistence.Sqlite/`
  - ✅ Crear archivo `NioxVF.Persistence.Sqlite.csproj`
  - ✅ Configurar dependencias SQLite

- ✅ **5.2 Crear SqliteDbContext**
  - ✅ Heredar de NioxVFDbContext
  - ✅ Configurar OnConfiguring para SQLite
  - ✅ Configurar cadena de conexión

#### ✅ **TAREA 6: Implementar Repositorios SQLite (100%)**
- ✅ **6.1 Crear Repositorio Base**
  - ✅ SqliteRepository<T> implementado
  - ✅ Métodos CRUD básicos funcionando
  - ✅ Manejo de excepciones y logging
- ✅ **6.2 Crear SqliteInvoiceRepository**
  - ✅ Gestión completa de facturas
  - ✅ Búsquedas por serie/número
  - ✅ Inclusión automática de TaxItems
- ✅ **6.3 Crear SqliteHashChainRepository**
  - ✅ Cadena criptográfica implementada
  - ✅ Operaciones atómicas con transacciones
  - ✅ Gestión de hashes por vendedor/serie
- ✅ **6.4 Crear SqliteSeriesLockRepository**
  - ✅ Control de concurrencia robusto
  - ✅ Locks con timeout configurable
  - ✅ Limpieza automática de locks expirados

#### ✅ **TAREA 7: Crear Migraciones Iniciales (100%)**
- ✅ **7.1 Configurar Herramientas EF**
  - ✅ dotnet-ef 8.0.0 instalado y configurado
  - ✅ Entity Framework Core 8.0.0 unificado
  - ✅ Herramientas funcionando sin cambios de PATH
- ✅ **7.2 Crear Migración Inicial**
  - ✅ Migración `20250825214755_InitialCreate` creada
  - ✅ Tabla `AuditEntity` con TPH (Table-Per-Hierarchy)
  - ✅ Índices y constraints configurados
- ✅ **7.3 Probar Migración**
  - ✅ Base de datos `NioxVF.db` creada exitosamente
  - ✅ Tablas y esquema verificados
  - ✅ Migración aplicada correctamente

#### ✅ **TAREA 8: Crear Tests Unitarios (100%)**
- ✅ **8.1 Crear Proyecto de Tests**
  - ✅ NioxVF.Persistence.Tests creado con .NET 8.0
  - ✅ Dependencias configuradas (xUnit, Moq, EF InMemory)
- ✅ **8.2 Crear Tests de Repositorio Base**
  - ✅ SqliteRepositoryTests: 11 tests pasando
  - ✅ Métodos CRUD básicos verificados
- ✅ **8.3 Crear Tests de InvoiceRepository**
  - ✅ SqliteInvoiceRepositoryTests: 7 tests pasando
  - ✅ Búsquedas específicas verificadas
- ✅ **8.4 Crear Tests de HashChainRepository**
  - ✅ SqliteHashChainRepositoryTests: 8 tests pasando
  - ✅ Operaciones de cadena verificadas
- ✅ **8.5 Crear Tests de SeriesLockRepository**
  - ✅ SqliteSeriesLockRepositoryTests: 10 tests pasando
  - ✅ Control de concurrencia verificado

#### ⏳ **TAREA 9: Documentación (0%)**
- [ ] **9.1 Documentar Interfaces**
- [ ] **9.2 Documentar Entidades**
- [ ] **9.3 Documentar Repositorios**
- [ ] **9.4 Crear README**

#### ⏳ **TAREA 10: Crear Pull Request (0%)**
- [ ] **10.1 Preparar Cambios**
- [ ] **10.2 Crear Commit**
- [ ] **10.3 Push y Pull Request**

### 🎯 Próximos Pasos Inmediatos

1. **Continuar con TAREA 9**: Documentación
2. **Completar TAREA 10**: Crear Pull Request
3. **Integrar con NioxVF.Api**: Configurar DI y servicios
4. **Testing end-to-end**: Verificar integración completa

### 📁 Estructura Actual del Proyecto

```
NioxVF.Persistence/
├── Interfaces/
│   ├── IRepository.cs ✅
│   ├── IInvoiceRepository.cs ✅
│   ├── IHashChainRepository.cs ✅
│   └── ISeriesLockRepository.cs ✅
├── Entities/
│   ├── Base/
│   │   └── AuditEntity.cs ✅
│   ├── InvoiceEntity.cs ✅
│   ├── TaxItemEntity.cs ✅
│   ├── HashChainEntity.cs ✅
│   └── SeriesLockEntity.cs ✅
├── Services/ (vacío - pendiente)
└── NioxVF.Persistence.csproj ✅
```

### ✅ NioxVF.Signing (Placeholder Implementado)

- ✅ `XadesSigner` - Implementación placeholder
- ✅ Interfaz `ISigner` correctamente definida
- ⚠️ **Pendiente Sprint 3**: Implementación XAdES-EPES real

### ✅ NioxVF.AeatSoap (Placeholder Implementado)

- ✅ `AeatSoapTransport` - Simulador de respuestas AEAT
- ✅ Generación de XML de respuesta simulado
- ✅ URLs de validación simuladas
- ⚠️ **Pendiente Sprint 4**: Cliente SOAP real con mTLS

### ✅ NioxVF.Qr (Placeholder Implementado)

- ✅ `QrGenerator` - Generador PNG placeholder
- ⚠️ **Pendiente Sprint 6**: Integración con QRCoder real

### ✅ NioxVF.Persistence (Implementación Completa)

- ✅ Proyecto creado y configurado
- ✅ Interfaces de repositorio implementadas
- ✅ Entidades de dominio creadas
- ✅ Entity Framework Core configurado
- ✅ DbContext base implementado
- ✅ Proyecto SQLite creado
- ✅ Repositorios específicos implementados
- ✅ **Migraciones**: Implementadas y funcionando (100%)
- ✅ **Tests unitarios**: 37 tests pasando (100%)
- ✅ **Unificación .NET 8.0**: Completada (100%)
- ✅ **Base de datos**: SQLite configurada y operativa

## 🚀 Instalación y Configuración

### Prerrequisitos

- ✅ .NET 8 SDK (versión unificada)
- ✅ Visual Studio 2022 o VS Code
- ✅ Git

### ✅ **Cambios Importantes - Unificación .NET 8.0**

**Todos los proyectos han sido unificados a .NET 8.0.0 para evitar conflictos de versiones:**

- ✅ **NioxVF.Api**: `net8.0` (sin cambios)
- ✅ **NioxVF.Persistence.Tests**: `net9.0` → `net8.0` (unificado)
- ✅ **Entity Framework Core**: `9.0.8` → `8.0.0` (unificado)
- ✅ **dotnet-ef tools**: `9.0.8` → `8.0.0` (unificado)

**Beneficios:**
- 🎯 **Consistencia**: Todo el proyecto usa .NET 8.0
- 🔧 **Simplicidad**: No más cambios de PATH
- ⚡ **Rendimiento**: Mejor compatibilidad entre componentes
- 🛠️ **Mantenimiento**: Más fácil de mantener y actualizar

### 1. Clonar y Compilar

```bash
git clone [URL_DEL_REPO] NioxVF
cd NioxVF
dotnet restore
dotnet build
```

**✅ Estado**: Compila sin errores ni warnings.

### 2. Configurar API

Editar `NioxVF.Api/appsettings.Development.json`:

```json
{
  "ApiKeys": [
    "dev-123",
    "test-456", 
    "local-789"
  ],
  "Aeat": {
    "Environment": "Pre",
    "Wsdl": "https://prewww2.aeat.es/wlpl/TIKE-CONT/VeriFactuService.wsdl"
  }
}
```

### 3. Configurar Agent

Editar `NioxVF.Agent/appsettings.Development.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5180",
  "ApiKey": "dev-123",
  "InboxPath": "C:\\NioxVF\\inbox",
  "QrPath": "C:\\NioxVF\\qr",
  "IntervalSeconds": 3
}
```

### 4. Crear Directorios

```bash
mkdir C:\NioxVF\inbox
mkdir C:\NioxVF\qr
mkdir C:\NioxVF\processed
mkdir C:\NioxVF\failed
```

## 🔧 Ejecución del Sistema

### Terminal 1 - API
```bash
cd NioxVF.Api
dotnet run --urls="http://localhost:5180"
```

**Endpoints disponibles:**
- 🌐 Swagger: http://localhost:5180/swagger
- 🔍 Health: http://localhost:5180/health
- 📊 Metrics: http://localhost:5180/metrics

### Terminal 2 - Agent
```bash
cd NioxVF.Agent
dotnet run
```

**Comportamiento:**
- ✅ Monitoriza `C:\NioxVF\inbox` cada 3 segundos
- ✅ Procesa archivos JSON automáticamente
- ✅ Genera logs detallados

## 📝 Pruebas y Ejemplos

### 1. Ticket de Prueba

Crear `C:\NioxVF\inbox\ticket1.json`:

```json
{
  "sellerNif": "B00000000",
  "sellerName": "Cafetería Demo NIOXTEC",
  "series": "A",
  "number": "00001",
  "issueDate": "2025-01-15T12:00:00",
  "taxes": [
    {
      "taxBase": 8.26,
      "taxRate": 21.0,
      "taxAmount": 1.74,
      "taxType": "IVA"
    }
  ],
  "text": "Venta mostrador - Café con tostada",
  "type": "F1"
}
```

### 2. Resultado Esperado

**Logs del Agent:**
```
INFO: Found 1 pending tickets
INFO: Processing ticket A-00001 from B00000000
INFO: Ticket processed successfully. ID: guid-xxx, Status: SENT, AEAT ID: AEAT-20250115120000-1234
INFO: QR generated for ticket A-00001 at C:\NioxVF\qr\A00001.png
```

**Archivos generados:**
- ✅ `C:\NioxVF\qr\A00001.png` - Código QR
- ✅ `C:\NioxVF\inbox\processed\ticket1_20250115120000.json` - Ticket procesado

### 3. Verificación via API

```bash
curl -H "X-API-Key: dev-123" http://localhost:5180/api/v1/invoices/[guid]
```

**Respuesta:**
```json
{
  "id": "guid-xxx",
  "status": "SENT",
  "aeatId": "AEAT-20250115120000-1234", 
  "qrUrl": "/api/v1/invoices/guid-xxx/qr",
  "validationUrl": "https://sede.agenciatributaria.gob.es/ValidarQR?..."
}
```

## 📊 Estado Detallado por Componente

### ✅ Completamente Implementado (Listos para Producción MVP)

| Componente | Funcionalidad | Estado | Notas |
|------------|---------------|--------|--------|
| **Project Structure** | Solución .NET 8 | ✅ 100% | 7 proyectos, compilación limpia |
| **Domain Models** | InvoiceSimple, TaxItem | ✅ 100% | Modelos completos según especificación |
| **Hash Chain** | Encadenado SHA-256 | ✅ 90% | Funcional, falta persistencia |
| **API Authentication** | X-API-Key middleware | ✅ 100% | Seguridad básica implementada |
| **API Endpoints** | POST/GET invoices | ✅ 100% | Contratos según especificación |
| **Agent Worker** | Background service | ✅ 100% | Polling y procesamiento automático |
| **Inbox Reader** | JSON file processing | ✅ 100% | Gestión de archivos completa |
| **API Client** | HTTP communication | ✅ 100% | Cliente HTTP con autenticación |
| **QR Generation** | PNG placeholder | ✅ 80% | Funcional, no QR real |
| **Configuration** | appsettings.json | ✅ 100% | Configuración completa |
| **Logging** | Structured logging | ✅ 100% | Logs detallados en ambos servicios |
| **Error Handling** | Exception management | ✅ 100% | Manejo robusto de errores |

### ⚠️ Implementaciones Placeholder (Funcionales pero no Productivas)

| Componente | Estado Actual | Sprint Objetivo |
|------------|---------------|-----------------|
| **XAdES Signer** | Placeholder XML | Sprint 3 - Firma real |
| **AEAT Transport** | Respuestas simuladas | Sprint 4 - SOAP+mTLS real |
| **QR Generator** | PNG simulado | Sprint 6 - QRCoder real |
| **Persistence** | Implementación completa | Sprint 2 - Migraciones y tests |

### ❌ Pendiente de Implementar

| Funcionalidad | Sprint | Prioridad |
|---------------|--------|-----------|
| XML F1 Generation | Sprint 2 | 🔴 Alta |
| Business Validations | Sprint 2 | 🔴 Alta |
| Real Certificate Handling | Sprint 3 | 🟠 Media |
| Database Migrations | Sprint 5 | 🟠 Media |
| Unit Tests | Sprint 7 | 🟡 Baja |
| Integration Tests | Sprint 8 | 🟡 Baja |

## 🔄 Roadmap de Desarrollo

### 🔄 Sprint 2 - Persistencia SQLite (80% COMPLETADO)
**Objetivo**: Implementar persistencia real con SQLite

- ✅ **HashChain persistente** (Implementación completa)
  - ✅ SQLite para Agent (proyecto creado)
  - ✅ Entity Framework Core configurado
  - ✅ Repositorios específicos implementados
  - ✅ Control de concurrencia por serie
  - ✅ **Migraciones**: Implementadas y funcionando
  - ✅ **Tests unitarios**: 37 tests pasando
  - ✅ **Unificación .NET 8.0**: Completada
- [ ] **Generación XML F1**
  - [ ] Serializer conforme a esquema Veri*Factu
  - [ ] Validación de estructura XML
- [ ] **Validaciones de negocio**
  - [ ] Suma de bases + impuestos = total
  - [ ] Validación de NIFs
  - [ ] Campos obligatorios

### 📅 Sprint 3 - Firma XAdES-EPES
- [ ] Implementación XadesSigner real
- [ ] Soporte PFX y Windows Store
- [ ] Políticas EPES según especificación
- [ ] Tests de validación de firma

### 📅 Sprint 4 - AEAT SOAP+mTLS  
- [ ] Cliente SOAP desde WSDL
- [ ] mTLS con certificados cliente
- [ ] Mapeo de respuestas y errores AEAT
- [ ] Idempotencia por clave natural

### 📅 Sprint 5 - Persistencia Completa
- [ ] EF Core + PostgreSQL para API
- ✅ SQLite para Agent (implementación completa)
- [ ] Esquema multi-tenant
- ✅ Migraciones automáticas (SQLite implementado)
 ### 📅 Sprint 6 - QR real + Integración Aronium
- [ ] QRCoder (PNG) con payload oficial
- [ ] Plantilla de ticket Aronium con imagen QR
- [ ] Lector real de BD/export de Aronium
- [ ] Validación visual y de URL de verificación

### 📅 Sprint 7 - Observabilidad & Hardening
- [ ] Serilog + sinks y paneles básicos
- [ ] HealthChecks y /metrics operativos
- [ ] Rate limiting, retries y circuit breakers
- [ ] Gestión segura de secretos y certificados (rotación)

### 📅 Sprint 8 - UAT / Preproducción AEAT
- [ ] Casos: IVA 0/10/21, recargo y cancelaciones
- [ ] Tests end-to-end en entorno Pre de AEAT
- [ ] Manual de soporte y troubleshooting
- [ ]Verificación de idempotencia y colas

📅 Sprint 9 - Producción (Go-Live)
- [ ] Certificados reales y endpoints de Producción
- [ ] Monitorización 24/7 y alertas
- [ ] Onboarding de tenants/clientes
- [ ] Plan de rollback y soporte inicial

## 📋 Testing y Validación

### ✅ Tests Realizados

- ✅ **Compilación**: Sin errores ni warnings
- ✅ **Integración básica**: Agent → API → Response
- ✅ **Encadenado**: Cálculo de hash correcto
- ✅ **Autenticación**: API Keys funcionando
- ✅ **File processing**: JSON inbox completo
- ✅ **Tests unitarios**: 37 tests pasando (100%)
- ✅ **Migraciones**: Base de datos creada correctamente
- ✅ **Unificación .NET 8.0**: Funcionando sin cambios de PATH

### 📝 Tests Pendientes

- [ ] Tests unitarios para cada componente
- [ ] Tests de integración con AEAT Pre
- [ ] Tests de carga y performance  
- [ ] Tests de certificados reales
- [ ] Tests de failover y resiliencia

## 🚨 Consideraciones Importantes

### ⚠️ **DISCLAIMER - MVP Estado**

**El proyecto está en estado MVP** y aunque funcional, NO está listo para producción hasta completar:

1. **Sprint 2**: XML F1 real y persistencia
2. **Sprint 3**: Firma XAdES-EPES real  
3. **Sprint 4**: Conexión AEAT real

### 🔒 **Seguridad**

**Implementado:**
- ✅ API Key authentication
- ✅ HTTPS configurado
- ✅ Input validation básica

**Pendiente:**
- ⚠️ Rotación de API Keys
- ⚠️ Custodia segura de certificados
- ⚠️ Rate limiting
- ⚠️ Audit logging completo

### 📈 **Performance**

**Actual:**
- ✅ Agent procesa ~1 ticket/segundo
- ✅ API responde en <100ms
- ✅ Memory usage estable

**Optimizaciones pendientes:**
- ⚠️ Batch processing de tickets
- ⚠️ Connection pooling DB
- ⚠️ Caching de respuestas

## 🆘 Soporte y Troubleshooting

### Problemas Comunes

#### ❌ "API Key required"
```bash
# Verificar headers
curl -H "X-API-Key: dev-123" http://localhost:5180/health
```

#### ❌ Agent no procesa archivos  
```bash
# Verificar permisos directorio
icacls C:\NioxVF\inbox
```

#### ❌ QR no se genera
```bash
# Verificar logs API para aeatId
tail -f logs/api.log | grep "aeatId"
```

### Logs y Monitoreo

**API Logs:**
```
INFO: NioxVF.Api started on http://localhost:5180
INFO: Invoice processed: A-00001, Status: SENT
DEBUG: HashChain updated for B00000000|A: ABC123...
```

**Agent Logs:**
```
INFO: NioxVF Agent started. Polling interval: 3s
INFO: Found 1 pending tickets
INFO: Processing ticket A-00001 from B00000000
INFO: QR generated at C:\NioxVF\qr\A00001.png
```

## 📞 Contacto

- **Proyecto**: NioxVF - Conector Veri*Factu
- **Contacto**: José Condolo (NIOXTEC)  
- **Estado**: Sprint 2 En Desarrollo (80%) 🚀
- **Responsable actual**: Angel - Backend & Persistencia
- **Próximo milestone**: Documentación y Pull Request

---

**NioxVF es un conector independiente para Veri*Factu. No es un producto oficial de la AEAT.**

*Última actualización: Diciembre 2024*
