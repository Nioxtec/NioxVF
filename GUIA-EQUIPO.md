# 👥 GUÍA PARA EL EQUIPO DE DESARROLLO - NioxVF

## 🛡️ **REGLAS DE ORO: Proteger el Sprint 1**

### ❌ **NUNCA HACER:**
- **NUNCA** trabajar directamente en la rama `main` 
- **NUNCA** hacer `git push origin main` sin revisión
- **NUNCA** eliminar el tag `v1.0-sprint1-stable`
- **NUNCA** hacer `git reset --hard` en `main`

### ✅ **SIEMPRE HACER:**
- **SIEMPRE** trabajar en ramas de desarrollo
- **SIEMPRE** hacer Pull Requests para cambios importantes
- **SIEMPRE** probar localmente antes de subir código
- **SIEMPRE** usar commits descriptivos

---

## 🚀 **FLUJO DE TRABAJO PARA EL EQUIPO**

### **PASO 1: Configurar entorno de trabajo**

```bash
# 1. Clonar el repositorio
git clone https://github.com/Nioxtec/NioxVF.git
cd NioxVF

# 2. Cambiar a la rama de desarrollo
git checkout sprint2-development

# 3. Crear tu rama personal
git checkout -b feature/tu-nombre-feature
# Ejemplo: git checkout -b feature/jose-xml-generation
```

### **PASO 2: Desarrollar tu feature**

```bash
# 1. Hacer cambios en tu rama
# - Editar código
# - Probar localmente
# - Verificar que compila: dotnet build

# 2. Commits frecuentes
git add .
git commit -m "feat: descripción clara de lo que hiciste"

# 3. Subir tu rama
git push origin feature/tu-nombre-feature
```

### **PASO 3: Integrar cambios**

```bash
# 1. Crear Pull Request en GitHub hacia sprint2-development
# 2. Esperar revisión y aprobación
# 3. Merge automático después de aprobación
```

---

## 🔄 **RAMAS Y SU PROPÓSITO**

### **🏆 main** 
- **Solo código estable y probado**
- **Sprint 1 completado y funcional**
- **PROTEGIDA: Solo admite merges via Pull Request**

### **🚧 sprint2-development**
- **Rama activa para Sprint 2**
- **Donde se integran todas las features**
- **Se fusiona a main cuando Sprint 2 esté completo**

### **⚡ feature/nombre-feature**
- **Ramas individuales para cada desarrollador**
- **Una rama por característica/bug**
- **Se elimina después del merge**

---

## 📋 **TAREAS SPRINT 2 - ASIGNACIÓN**

### **🎯 PRIORIDAD ALTA**

#### **👤 Desarrollador 1: Persistencia Real**
```bash
git checkout -b feature/persistencia-database
```
**Tareas:**
- [ ] Implementar EF Core para NioxVF.Api (PostgreSQL)
- [ ] Implementar SQLite para NioxVF.Agent  
- [ ] Crear migraciones
- [ ] Actualizar HashChainService para usar BD
- [ ] Tests de persistencia

#### **👤 Desarrollador 2: Generación XML F1**
```bash
git checkout -b feature/xml-verifactu
```
**Tareas:**
- [ ] Crear serializer XML conforme a esquema Veri*Factu
- [ ] Implementar validaciones de estructura XML
- [ ] Reemplazar placeholder en InvoiceService
- [ ] Tests de generación XML
- [ ] Validación contra XSD oficial

#### **👤 Desarrollador 3: Validaciones de Negocio**
```bash
git checkout -b feature/business-validations
```
**Tareas:**
- [ ] Validación: suma bases + IVA = total
- [ ] Validación: formato NIF español
- [ ] Validación: campos obligatorios por tipo factura
- [ ] Validación: rangos de fechas permitidos
- [ ] Tests unitarios de validaciones

---

## 🧪 **PROTOCOLO DE TESTING**

### **Antes de hacer commit:**
```bash
# 1. Compilar sin errores
dotnet build

# 2. Ejecutar tests (cuando los tengamos)
dotnet test

# 3. Probar manualmente tu feature
# - Ejecutar API y Agent
# - Probar caso feliz y casos error
```

### **Antes de Pull Request:**
```bash
# 1. Sincronizar con sprint2-development
git checkout sprint2-development
git pull origin sprint2-development
git checkout feature/tu-rama
git merge sprint2-development

# 2. Resolver conflictos si los hay
# 3. Probar que todo sigue funcionando
# 4. Subir y crear PR
```

---

## 🆘 **RECUPERACIÓN DE EMERGENCIA**

### **Si algo se rompe en sprint2-development:**
```bash
# Volver al punto estable
git checkout main
git checkout -b hotfix/recover-from-stable
git push origin hotfix/recover-from-stable
```

### **Si necesitas el Sprint 1 original:**
```bash
# Ir al tag estable
git checkout v1.0-sprint1-stable
git checkout -b recovery/sprint1-stable
```

---

## 📞 **COMUNICACIÓN DEL EQUIPO**

### **🔄 Daily Meetings:**
- ¿En qué rama estás trabajando?
- ¿Qué feature estás implementando?
- ¿Tienes bloqueos o conflictos?
- ¿Necesitas hacer merge?

### **📋 Pull Request Protocol:**
1. **Título claro**: `feat: Implementar persistencia PostgreSQL`
2. **Descripción detallada**: Qué cambios hiciste y por qué
3. **Tests**: Cómo probaste los cambios
4. **Screenshots**: Si cambió la UI o logs

### **🚨 Emergencias:**
- Contactar al líder técnico inmediatamente
- NO hacer cambios en `main` sin autorización
- Documentar el problema en issue de GitHub

---

## 🎯 **OBJETIVOS SPRINT 2**

### **✅ Criterios de Aceptación:**
- [ ] Persistencia real funcionando (BD)
- [ ] XML F1 conforme a esquema oficial AEAT
- [ ] Validaciones de negocio completas
- [ ] Tests unitarios para componentes nuevos
- [ ] Integración E2E funcionando como Sprint 1
- [ ] Documentación actualizada

### **🏁 Definición de "Terminado":**
- ✅ Código revisado y aprobado
- ✅ Tests pasando
- ✅ Documentación actualizada
- ✅ Integrado en sprint2-development
- ✅ Probado en entorno local
- ✅ Sin regresiones del Sprint 1

---

## 💡 **CONSEJOS PARA EL EQUIPO**

### **🎯 Mejores Prácticas:**
- **Commits pequeños y frecuentes** mejor que commits gigantes
- **Probar localmente** antes de subir código
- **Pedir ayuda** si algo no está claro
- **Documentar decisiones técnicas** importantes

### **🔧 Herramientas Recomendadas:**
- **Visual Studio 2022** o **VS Code** con extensiones C#
- **Git GUI** como GitKraken o SourceTree (opcional)
- **Postman** para probar la API
- **PostgreSQL** local para development

### **📚 Recursos:**
- **Documentación oficial .NET 8**: https://docs.microsoft.com/dotnet
- **Esquema Veri*Factu AEAT**: [enlace al XSD oficial]
- **README del proyecto**: Información completa del sistema

---

**🎉 ¡Trabajemos juntos para hacer un Sprint 2 exitoso sin romper el Sprint 1!**

*Última actualización: Diciembre 2024*
