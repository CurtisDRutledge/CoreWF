# .NET 10 Upgrade and Solution Migration Plan

**Date**: January 2025  
**Solution**: Quorum.CoreWF  
**Current Framework**: .NET 6.0  
**Target Framework**: .NET 10.0 (Long Term Support)  
**Projects**: 17  
**Additional Goal**: Migrate solution from .sln to .slnx format

---

## Executive Summary

This plan details the migration of the Quorum.CoreWF solution from .NET 6.0 to .NET 10.0, involving 17 projects with a complex dependency hierarchy. The migration includes updating the target framework across all projects, upgrading 11 NuGet packages, addressing 3 deprecated packages, and converting the solution file from legacy .sln to modern .slnx format.

**Key Highlights:**
- **17 projects** to upgrade across 5 dependency levels
- **2,487 total issues** identified (17 mandatory, 2,450 potential, 3 optional)
- **11 package updates** recommended for .NET 10 compatibility
- **3 deprecated packages** to replace (Azure.Identity, Microsoft.Azure.ServiceBus, Microsoft.Azure.Storage.Blob)
- **75 files** affected by API compatibility changes
- **Solution format migration** from .sln to .slnx
- **No security vulnerabilities** detected ?
- **In-place upgrade** on `develop` branch (no separate upgrade branch)

**Migration Strategy Selected**: **Incremental Migration** (dependency-driven, bottom-up approach)

**Rationale**: With 17 projects across 5 dependency levels and 2,487 issues to address, an incremental approach reduces risk by allowing validation at each dependency level before proceeding. This ensures each layer is stable before dependent projects are migrated.

---

## Migration Strategy

### Approach: Incremental Migration by Dependency Level

We will migrate projects in **5 phases**, following the dependency graph from foundation projects (Level 0) to top-level applications (Level 4). This bottom-up approach ensures:

1. **Lower dependencies are stable** before higher-level projects migrate
2. **Issues are isolated** to specific layers
3. **Testing can be progressive** with validation checkpoints
4. **Rollback is manageable** if issues arise at any level

### Migration Order Phases

**Phase 1 (Level 0)**: Foundation library - 1 project
- Quorum.CoreWF.Xaml (no dependencies)

**Phase 2 (Level 1)**: Core runtime libraries - 2 projects
- Microsoft.CodeAnalysis.VisualBasic.Scripting
- Quorum.CoreWF.Runtime

**Phase 3 (Level 2)**: Extended libraries and simple test projects - 6 projects
- CustomTestObjects
- ImperativeTestCases
- Quorum.CoreWF.Core
- Quorum.CoreWF.EtwTracking
- System.Xaml.TestCases
- TestObjects
- WorkflowApplicationTestExtensions

**Phase 4 (Level 3)**: Advanced test projects and benchmarks - 6 projects
- CoreWf.Benchmarks
- Perf.AssemblyReference.Benchmarks
- TestCases.Activities
- TestCases.Runtime
- TestCases.Workflows
- TestCases.Xaml

**Phase 5 (Level 4)**: Top-level applications - 1 project
- TestConsole

**Phase 6**: Solution file migration (.sln to .slnx)

---

## Dependency Analysis

### Project Dependency Hierarchy

```
Level 0 (Foundation):
??? Quorum.CoreWF.Xaml (97 issues, 1 mandatory)
    ?
    ??? Level 1:
    ?   ??? Microsoft.CodeAnalysis.VisualBasic.Scripting (2 issues, 1 mandatory)
    ?   ??? Quorum.CoreWF.Runtime (25 issues, 1 mandatory)
    ?       ?
    ?       ??? Level 2:
    ?       ?   ??? CustomTestObjects (3 issues, 1 mandatory)
    ?       ?   ??? ImperativeTestCases (3 issues, 1 mandatory)
    ?       ?   ??? Quorum.CoreWF.Core (2,123 issues, 1 mandatory)
    ?       ?   ??? Quorum.CoreWF.EtwTracking (3 issues, 1 mandatory)
    ?       ?   ??? System.Xaml.TestCases (20 issues, 1 mandatory)
    ?       ?   ??? TestObjects (94 issues, 1 mandatory)
    ?       ?   ??? WorkflowApplicationTestExtensions (4 issues, 1 mandatory)
    ?       ?       ?
    ?       ?       ??? Level 3:
    ?       ?       ?   ??? CoreWf.Benchmarks (2 issues, 1 mandatory)
    ?       ?       ?   ??? Perf.AssemblyReference.Benchmarks (22 issues, 1 mandatory)
    ?       ?       ?   ??? TestCases.Activities (5 issues, 1 mandatory)
    ?       ?       ?   ??? TestCases.Runtime (16 issues, 1 mandatory)
    ?       ?       ?   ??? TestCases.Workflows (56 issues, 1 mandatory)
    ?       ?       ?   ??? TestCases.Xaml (9 issues, 1 mandatory)
    ?       ?       ?       ?
    ?       ?       ?       ??? Level 4:
    ?       ?       ?           ??? TestConsole (3 issues, 1 mandatory)
```

### Critical Path Projects

**Quorum.CoreWF.Xaml** ? Foundation for entire solution (16 projects depend on it)  
**Quorum.CoreWF.Runtime** ? Core runtime (14 projects depend on it)  
**Quorum.CoreWF.Core** ? Primary library (2,123 issues - **highest complexity**)

---

## Package Update Reference

### Required Package Updates (11 packages)

| Package Name | Current Version | Target Version | Affected Projects | Reason |
|-------------|-----------------|----------------|-------------------|---------|
| Microsoft.AspNetCore.Authorization | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |
| Microsoft.Extensions.Configuration | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |
| Microsoft.Extensions.Configuration.Json | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |
| Microsoft.Extensions.DependencyInjection | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |
| Microsoft.Extensions.Logging | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |
| Microsoft.Extensions.Options | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |
| Newtonsoft.Json | 13.0.2 / 13.0.3 | 13.0.4 | Multiple | Latest stable version |
| System.CodeDom | 6.0.0 | 10.0.2 | Multiple | .NET 10 compatibility |
| System.Drawing.Common | 6.0.0 | 10.0.2 | Multiple | .NET 10 compatibility |
| System.IO.Pipelines | 9.0.6 | 10.0.2 | Multiple | .NET 10 compatibility |

### Deprecated Packages to Replace (3 packages)

| Deprecated Package | Current Version | Recommended Action | Affected Project |
|-------------------|-----------------|-------------------|------------------|
| Azure.Identity | 1.14.0 | Monitor deprecation notices; plan migration | TBD |
| Microsoft.Azure.ServiceBus | 5.2.0 | Migrate to Azure.Messaging.ServiceBus | TBD |
| Microsoft.Azure.Storage.Blob | 11.2.3 | Migrate to Azure.Storage.Blobs | TBD |

**Note**: Deprecated packages should be addressed in Phase 3+ or as a follow-up task depending on usage patterns.

---

## Breaking Changes Catalog

### .NET 6 ? .NET 10 Breaking Changes

Based on the assessment, the following categories of breaking changes have been identified:

#### 1. API Incompatibilities (Api.0002)
**Impact**: 11 projects affected, 75 files  
**Description**: API methods, properties, or types that are source-incompatible with .NET 10

**Expected Changes**:
- Obsolete API usage needs replacement
- Method signature changes (parameters, return types)
- Type relocations or renames
- Property accessibility changes

**Discovery Approach**: Compilation errors will reveal specific incompatibilities after framework upgrade.

#### 2. Behavioral Changes (Api.0003)
**Impact**: 5 projects affected  
**Affected Projects**:
- Quorum.CoreWF.Xaml
- Quorum.CoreWF.Core
- Perf.AssemblyReference.Benchmarks
- TestCases.Xaml
- System.Xaml.TestCases
- TestCases.Activities

**Description**: APIs that compile but behave differently in .NET 10 compared to .NET 6

**Expected Changes**:
- Runtime behavior modifications
- Default value changes
- Performance characteristics
- Exception handling differences

**Mitigation**: Thorough testing, especially for edge cases and boundary conditions.

#### 3. CodeDom & Dynamic Code Generation Issues
**Impact**: 2,164 issues across multiple projects  
**Primary Affected Projects**:
- Quorum.CoreWF.Core (contains bulk of issues)
- Related test projects

**Description**: Changes in System.CodeDom and dynamic code generation patterns

**Expected Changes**:
- CodeDom API updates required
- Dynamic compilation pattern adjustments
- Reflection-based code may need updates

---

## Phase-by-Phase Migration Plan

### Phase 1: Foundation Layer (Level 0)

**Project**: Quorum.CoreWF.Xaml  
**Dependencies**: None  
**Issue Count**: 97 (1 mandatory)  
**Risk Level**: **High** (foundation for 16 other projects)

#### Steps:

1. **Update Project File**
   - [ ] Open `Quorum.CoreWF.Xaml.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Save changes

2. **Build and Address Compilation Errors**
   - [ ] Build project: `dotnet build src\Quorum.CoreWF.Xaml\Quorum.CoreWF.Xaml.csproj`
   - [ ] Review compilation errors
   - [ ] Address API incompatibilities:
     - Replace obsolete APIs with recommended alternatives
     - Update method signatures
     - Resolve namespace changes
   - [ ] Repeat build until no errors

3. **Address Build Warnings**
   - [ ] Review all warnings
   - [ ] Fix warnings related to:
     - Obsolete API usage
     - Nullable reference types
     - Code analysis suggestions
   - [ ] Target: zero warnings

4. **Testing**
   - [ ] Build succeeds without errors
   - [ ] Build succeeds without warnings
   - [ ] Visual inspection of XAML-related functionality (if applicable)

5. **Commit Changes**
   - [ ] Commit framework upgrade: `git commit -m "Upgrade Quorum.CoreWF.Xaml to .NET 10"`

**Estimated Time**: 2-3 hours  
**Success Criteria**: Clean build, no warnings, all tests pass

---

### Phase 2: Core Runtime Libraries (Level 1)

#### 2A. Microsoft.CodeAnalysis.VisualBasic.Scripting

**Dependencies**: Quorum.CoreWF.Xaml  
**Issue Count**: 2 (1 mandatory)  
**Risk Level**: **Medium**

##### Steps:

1. **Update Project File**
   - [ ] Open `Microsoft.CodeAnalysis.VisualBasic.Scripting.vbproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address any VB.NET-specific API changes
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings
   - [ ] Scripting functionality verified

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade Microsoft.CodeAnalysis.VisualBasic.Scripting to .NET 10"`

**Estimated Time**: 1 hour

---

#### 2B. Quorum.CoreWF.Runtime

**Dependencies**: Quorum.CoreWF.Xaml  
**Issue Count**: 25 (1 mandatory)  
**Risk Level**: **High** (core runtime, 14 projects depend on it)

##### Steps:

1. **Update Project File**
   - [ ] Open `Quorum.CoreWF.Runtime.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities (25 issues expected)
   - [ ] Focus on runtime-critical APIs
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings
   - [ ] Unit tests pass (if available)
   - [ ] Runtime behavior validated

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade Quorum.CoreWF.Runtime to .NET 10"`

**Estimated Time**: 3-4 hours  
**Success Criteria**: Clean build, all runtime tests pass

---

### Phase 3: Extended Libraries and Test Infrastructure (Level 2)

This phase includes 7 projects. Order within phase can be adjusted based on complexity.

#### 3A. Quorum.CoreWF.Core

**Dependencies**: Quorum.CoreWF.Xaml, Microsoft.CodeAnalysis.VisualBasic.Scripting, Quorum.CoreWF.Runtime  
**Issue Count**: **2,123 (1 mandatory)** - **HIGHEST COMPLEXITY**  
**Risk Level**: **Critical**

##### Steps:

1. **Update Project File**
   - [ ] Open `Quorum.CoreWF.Core.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references:
     - [ ] `Microsoft.Extensions.Configuration` ? 10.0.2
     - [ ] `Microsoft.Extensions.Configuration.Json` ? 10.0.2
     - [ ] `Microsoft.Extensions.DependencyInjection` ? 10.0.2
     - [ ] `Microsoft.Extensions.Logging` ? 10.0.2
     - [ ] `Microsoft.Extensions.Options` ? 10.0.2
     - [ ] `System.CodeDom` ? 10.0.2
     - [ ] `System.Drawing.Common` ? 10.0.2
     - [ ] Any version of `Newtonsoft.Json` ? 13.0.4
   - [ ] Save changes

2. **Build and Address CodeDom Issues**
   - [ ] Build project
   - [ ] **Focus Area**: CodeDom & Dynamic Code Generation (2,164 issues in feature)
   - [ ] Address System.CodeDom API changes:
     - Review CodeDOM compilation patterns
     - Update dynamic code generation logic
     - Adjust reflection-based code
   - [ ] Resolve API incompatibilities
   - [ ] Fix behavioral change impacts
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings
   - [ ] Unit tests pass
   - [ ] Dynamic code generation validated
   - [ ] CodeDom functionality tested

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade Quorum.CoreWF.Core to .NET 10 with CodeDom updates"`

**Estimated Time**: 8-12 hours (due to high issue count)  
**Success Criteria**: Clean build, all tests pass, CodeDom features functional

---

#### 3B. Quorum.CoreWF.EtwTracking

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime  
**Issue Count**: 3 (1 mandatory)  
**Risk Level**: **Low**

##### Steps:

1. **Update Project File**
   - [ ] Open `Quorum.CoreWF.EtwTracking.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings
   - [ ] ETW tracking functionality verified

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade Quorum.CoreWF.EtwTracking to .NET 10"`

**Estimated Time**: 1 hour

---

#### 3C. TestObjects

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime  
**Issue Count**: 94 (1 mandatory)  
**Risk Level**: **Medium** (used by multiple test projects)

##### Steps:

1. **Update Project File**
   - [ ] Open `TestObjects.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities (94 issues)
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade TestObjects to .NET 10"`

**Estimated Time**: 2-3 hours

---

#### 3D. WorkflowApplicationTestExtensions

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime  
**Issue Count**: 4 (1 mandatory)  
**Risk Level**: **Medium** (used by multiple test projects)

##### Steps:

1. **Update Project File**
   - [ ] Open `WorkflowApplicationTestExtensions.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade WorkflowApplicationTestExtensions to .NET 10"`

**Estimated Time**: 1 hour

---

#### 3E. CustomTestObjects

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime  
**Issue Count**: 3 (1 mandatory)  
**Risk Level**: **Low**

##### Steps:

1. **Update Project File**
   - [ ] Open `CustomTestObjects.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] No warnings

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade CustomTestObjects to .NET 10"`

**Estimated Time**: 1 hour

---

#### 3F. ImperativeTestCases

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime  
**Issue Count**: 3 (1 mandatory)  
**Risk Level**: **Low**

##### Steps:

1. **Update Project File**
   - [ ] Open `ImperativeTestCases.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address compilation errors
   - [ ] Eliminate warnings

3. **Run Tests**
   - [ ] Execute test suite
   - [ ] Verify all tests pass

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade ImperativeTestCases to .NET 10"`

**Estimated Time**: 1 hour

---

#### 3G. System.Xaml.TestCases

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime  
**Issue Count**: 20 (1 mandatory)  
**Risk Level**: **Medium**

##### Steps:

1. **Update Project File**
   - [ ] Open `System.Xaml.TestCases.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities and behavioral changes
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Run Tests**
   - [ ] Execute test suite
   - [ ] Verify all tests pass
   - [ ] Check for behavioral change impacts

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade System.Xaml.TestCases to .NET 10"`

**Estimated Time**: 2 hours

---

### Phase 4: Advanced Test Projects and Benchmarks (Level 3)

This phase includes 6 projects focused on testing and performance benchmarking.

#### 4A. TestCases.Activities

**Dependencies**: Quorum.CoreWF.Xaml, WorkflowApplicationTestExtensions, TestObjects, Quorum.CoreWF.Runtime  
**Issue Count**: 5 (1 mandatory)  
**Risk Level**: **Medium**

##### Steps:

1. **Update Project File**
   - [ ] Open `TestCases.Activities.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address behavioral changes
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Run Tests**
   - [ ] Execute test suite
   - [ ] Verify all tests pass

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade TestCases.Activities to .NET 10"`

**Estimated Time**: 1-2 hours

---

#### 4B. TestCases.Runtime

**Dependencies**: Quorum.CoreWF.Xaml, WorkflowApplicationTestExtensions, TestObjects, Quorum.CoreWF.Runtime  
**Issue Count**: 16 (1 mandatory)  
**Risk Level**: **Medium**

##### Steps:

1. **Update Project File**
   - [ ] Open `TestCases.Runtime.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Run Tests**
   - [ ] Execute test suite
   - [ ] Verify all tests pass
   - [ ] Focus on runtime behavior validation

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade TestCases.Runtime to .NET 10"`

**Estimated Time**: 2 hours

---

#### 4C. TestCases.Workflows

**Dependencies**: Quorum.CoreWF.Xaml, WorkflowApplicationTestExtensions, Quorum.CoreWF.Runtime, Quorum.CoreWF.Core, CustomTestObjects  
**Issue Count**: 56 (1 mandatory)  
**Risk Level**: **High**

##### Steps:

1. **Update Project File**
   - [ ] Open `TestCases.Workflows.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities (56 issues)
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Run Tests**
   - [ ] Execute test suite
   - [ ] Verify all tests pass
   - [ ] Validate workflow execution patterns

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade TestCases.Workflows to .NET 10"`

**Estimated Time**: 3-4 hours

---

#### 4D. TestCases.Xaml

**Dependencies**: Quorum.CoreWF.Xaml, TestObjects, Quorum.CoreWF.Runtime, Quorum.CoreWF.Core  
**Issue Count**: 9 (1 mandatory)  
**Risk Level**: **Medium**

##### Steps:

1. **Update Project File**
   - [ ] Open `TestCases.Xaml.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities and behavioral changes
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Run Tests**
   - [ ] Execute test suite
   - [ ] Verify all tests pass

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade TestCases.Xaml to .NET 10"`

**Estimated Time**: 1-2 hours

---

#### 4E. CoreWf.Benchmarks

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime, Quorum.CoreWF.Core  
**Issue Count**: 2 (1 mandatory)  
**Risk Level**: **Low**

##### Steps:

1. **Update Project File**
   - [ ] Open `CoreWf.Benchmarks.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] Run sample benchmarks to verify functionality

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade CoreWf.Benchmarks to .NET 10"`

**Estimated Time**: 1 hour

---

#### 4F. Perf.AssemblyReference.Benchmarks

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime, Quorum.CoreWF.Core  
**Issue Count**: 22 (1 mandatory)  
**Risk Level**: **Medium**

##### Steps:

1. **Update Project File**
   - [ ] Open `Perf.AssemblyReference.Benchmarks.csproj`
   - [ ] Change `<TargetFramework>net6.0;net6.0-windows</TargetFramework>` to `<TargetFramework>net10.0;net10.0-windows</TargetFramework>`
   - [ ] **Address deprecated package**: Plan migration from deprecated packages (noted in assessment)
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address API incompatibilities and behavioral changes
   - [ ] Resolve compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] Run sample benchmarks to verify functionality

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade Perf.AssemblyReference.Benchmarks to .NET 10"`

**Estimated Time**: 2 hours

---

### Phase 5: Top-Level Application (Level 4)

#### 5A. TestConsole

**Dependencies**: Quorum.CoreWF.Xaml, Quorum.CoreWF.Runtime, TestCases.Runtime  
**Issue Count**: 3 (1 mandatory)  
**Risk Level**: **Low**

##### Steps:

1. **Update Project File**
   - [ ] Open `TestConsole.csproj`
   - [ ] Change `<TargetFramework>net6.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`
   - [ ] Update package references if needed
   - [ ] Save changes

2. **Build and Fix**
   - [ ] Build project
   - [ ] Address compilation errors
   - [ ] Eliminate warnings

3. **Testing**
   - [ ] Build succeeds
   - [ ] Run console application
   - [ ] Verify expected functionality

4. **Commit**
   - [ ] Commit: `git commit -m "Upgrade TestConsole to .NET 10"`

**Estimated Time**: 1 hour

---

### Phase 6: Solution File Migration (.sln to .slnx)

**Dependencies**: All projects successfully migrated to .NET 10  
**Risk Level**: **Medium** (affects entire solution structure)

#### Background

The `.slnx` format is the new XML-based solution file format introduced in Visual Studio 2022. It provides:
- **Human-readable XML structure** (easier to review in source control)
- **Better merge conflict resolution** (clearer structure than .sln)
- **Modern solution management** (improved tooling support)

#### Steps:

1. **Verify All Projects Built Successfully**
   - [ ] Ensure all 17 projects build without errors
   - [ ] Confirm no warnings remain
   - [ ] Run full solution build: `dotnet build src\Quorum.CoreWF.sln`

2. **Backup Current Solution File**
   - [ ] Create backup: `copy src\Quorum.CoreWF.sln src\Quorum.CoreWF.sln.backup`

3. **Convert Solution to .slnx Format**
   
   **Option A: Using Visual Studio 2022**
   - [ ] Open `Quorum.CoreWF.sln` in Visual Studio 2022
   - [ ] Go to File ? Save As
   - [ ] Select ".slnx" format
   - [ ] Save as `Quorum.CoreWF.slnx` in same directory
   
   **Option B: Using dotnet CLI** (if supported)
   - [ ] Run: `dotnet sln migrate src\Quorum.CoreWF.sln --to-slnx`
   
   **Option C: Manual Migration**
   - [ ] Use solution conversion tool or script
   - [ ] Verify all project references are preserved

4. **Validate New Solution File**
   - [ ] Open `Quorum.CoreWF.slnx` in Visual Studio 2022
   - [ ] Verify all 17 projects are loaded correctly
   - [ ] Verify solution folders are preserved (Test, Perf, etc.)
   - [ ] Check configuration mappings (Debug, Release)

5. **Build with New Solution Format**
   - [ ] Close Visual Studio
   - [ ] Build solution: `dotnet build src\Quorum.CoreWF.slnx`
   - [ ] Verify successful build
   - [ ] Confirm all projects compile

6. **Update Repository References**
   - [ ] Update any documentation referring to .sln file
   - [ ] Update CI/CD pipelines to use .slnx
   - [ ] Update README.md if it references solution file
   - [ ] Update any build scripts

7. **Remove Old Solution File**
   - [ ] Delete `src\Quorum.CoreWF.sln`
   - [ ] Keep backup temporarily (remove after validation)

8. **Testing**
   - [ ] Build entire solution
   - [ ] Run all test projects
   - [ ] Verify IDE functionality (IntelliSense, debugging, etc.)
   - [ ] Test solution in different development environments

9. **Commit Changes**
   - [ ] Commit: `git commit -m "Migrate solution from .sln to .slnx format"`

**Estimated Time**: 2-3 hours  
**Success Criteria**: All projects load and build correctly with .slnx format

---

## Testing Strategy

### Multi-Level Testing Approach

Testing occurs at three levels throughout the migration:

#### Level 1: Per-Project Testing (After Each Project Migration)

**For Every Project:**
- [ ] **Build Validation**
  - No compilation errors
  - Zero warnings (or documented acceptable warnings)
  - Package restore succeeds
  
- [ ] **Project-Specific Tests**
  - Unit tests pass (if project has tests)
  - Project-specific functionality verified
  
- [ ] **Dependency Validation**
  - Projects depending on this one still build
  - No dependency conflicts introduced

#### Level 2: Phase Testing (After Each Dependency Level)

**After Each Phase:**
- [ ] **Integration Build**
  - All projects in phase build together
  - All projects in previous phases still build
  - Full solution build succeeds up to this phase
  
- [ ] **Integration Testing**
  - Cross-project functionality works
  - Dependencies between phase projects validated
  - No regression in previously tested phases
  
- [ ] **Test Suite Execution**
  - Run all test projects migrated so far
  - Validate test results against baseline
  - Document any expected behavioral differences

#### Level 3: Full Solution Testing (After All Projects Migrated)

**Final Validation:**
- [ ] **Complete Solution Build**
  - `dotnet build src\Quorum.CoreWF.slnx` succeeds
  - All configurations build (Debug, Release)
  - All platforms build (if multi-platform)
  
- [ ] **Comprehensive Test Execution**
  - Run all test projects:
    - ImperativeTestCases
    - System.Xaml.TestCases
    - TestCases.Activities
    - TestCases.Runtime
    - TestCases.Workflows
    - TestCases.Xaml
  - Document pass/fail results
  - Investigate any failures
  
- [ ] **Performance Validation**
  - Run CoreWf.Benchmarks
  - Run Perf.AssemblyReference.Benchmarks
  - Compare against .NET 6 baseline (if available)
  - Document performance changes
  
- [ ] **End-to-End Scenarios**
  - Run TestConsole application
  - Execute key workflow scenarios
  - Validate XAML processing
  - Verify runtime behavior
  
- [ ] **Quality Checks**
  - No package dependency conflicts
  - No security vulnerabilities (run security scan)
  - Code analysis clean
  - Documentation updated

### Test Failure Protocol

**If Tests Fail:**

1. **Isolate the Issue**
   - Determine which project/phase introduced the failure
   - Identify if it's a build error or runtime failure
   
2. **Categorize the Failure**
   - API breaking change
   - Behavioral change
   - Test framework issue
   - Environment-specific issue
   
3. **Resolution Path**
   - For API changes: Update code to use .NET 10 APIs
   - For behavioral changes: Adjust code or update test expectations
   - For test framework issues: Update test project dependencies
   
4. **Validation**
   - Re-run tests after fix
   - Verify no new failures introduced
   - Update documentation with changes made

---

## Risk Management

### Identified Risks and Mitigation

#### Risk 1: High Issue Count in Quorum.CoreWF.Core

**Description**: 2,123 issues in a single project, primarily related to CodeDom  
**Likelihood**: High  
**Impact**: High

**Mitigation**:
- Allocate extended time for this project (8-12 hours estimated)
- Break migration into sub-tasks (CodeDom first, then other APIs)
- Conduct incremental testing after each sub-task
- Have rollback plan ready (git commit after each sub-task)
- Consider pairing/code review for complex CodeDom changes

**Rollback**: `git reset --hard` to commit before Quorum.CoreWF.Core migration

---

#### Risk 2: Behavioral Changes in XAML Projects

**Description**: 5 projects have behavioral changes that may not surface during compilation  
**Likelihood**: Medium  
**Impact**: High

**Affected Projects**:
- Quorum.CoreWF.Xaml
- Quorum.CoreWF.Core
- Perf.AssemblyReference.Benchmarks
- TestCases.Xaml
- System.Xaml.TestCases
- TestCases.Activities

**Mitigation**:
- Thorough test execution for each affected project
- Review .NET 10 behavioral change documentation
- Test edge cases and boundary conditions
- Compare runtime behavior against .NET 6 (if possible)
- Document expected behavioral differences

**Rollback**: Per-project rollback using git commits

---

#### Risk 3: Deprecated Package Dependencies

**Description**: 3 packages are deprecated and may cause future issues  
**Likelihood**: Medium  
**Impact**: Medium

**Packages**:
- Azure.Identity (1.14.0)
- Microsoft.Azure.ServiceBus (5.2.0)
- Microsoft.Azure.Storage.Blob (11.2.3)

**Mitigation**:
- Monitor deprecation timelines
- Plan migration to replacement packages:
  - Microsoft.Azure.ServiceBus ? Azure.Messaging.ServiceBus
  - Microsoft.Azure.Storage.Blob ? Azure.Storage.Blobs
- Document migration path for future work
- Consider addressing in separate follow-up migration

**Rollback**: N/A (deprecation is informational)

---

#### Risk 4: Multi-Target Framework in Perf.AssemblyReference.Benchmarks

**Description**: Project targets both `net6.0` and `net6.0-windows`  
**Likelihood**: Low  
**Impact**: Medium

**Mitigation**:
- Update both targets to `net10.0` and `net10.0-windows`
- Test on Windows to ensure Windows-specific functionality works
- Verify platform-specific APIs are compatible with .NET 10
- Test benchmarks on target platform

**Rollback**: Revert project file to previous targets

---

#### Risk 5: Solution Format Migration Issues

**Description**: Converting .sln to .slnx may cause tooling or CI/CD issues  
**Likelihood**: Low  
**Impact**: Medium

**Mitigation**:
- Perform migration as final step (after all projects stable)
- Keep backup of .sln file temporarily
- Update CI/CD pipelines before deleting .sln
- Test .slnx in multiple environments before committing
- Verify Visual Studio 2022 support

**Rollback**: Restore backed-up .sln file, delete .slnx

---

### Rollback Strategy

**Per-Phase Rollback:**
- Each phase has dedicated git commits
- Can rollback to any phase completion point
- Rollback command: `git reset --hard <commit-hash>`

**Full Migration Rollback:**
- Return to commit before Phase 1
- Command: `git reset --hard <initial-commit>`
- All projects revert to .NET 6

**Critical Rollback Points:**
1. Before Phase 1 (initial state)
2. After Phase 1 (foundation stable)
3. After Phase 2 (core runtime stable)
4. After Phase 3A (Quorum.CoreWF.Core stable - critical milestone)
5. After Phase 5 (all projects migrated, before .slnx)
6. After Phase 6 (complete migration)

---

## Success Criteria

### The migration is complete when:

#### Technical Criteria

- [ ] **All 17 projects target .NET 10.0**
  - TargetFramework element updated in all .csproj files
  - Multi-target projects updated (net10.0, net10.0-windows)

- [ ] **All recommended package updates applied**
  - 11 packages updated to .NET 10-compatible versions
  - Newtonsoft.Json updated to 13.0.4

- [ ] **All projects build successfully**
  - No compilation errors
  - Zero warnings (or documented acceptable warnings)
  - Package restore succeeds for all projects

- [ ] **All tests pass**
  - All test projects execute successfully
  - Test results match or exceed .NET 6 baseline
  - No unexplained test failures

- [ ] **Solution format migrated**
  - Solution converted from .sln to .slnx
  - All projects load correctly in new format
  - Build succeeds using .slnx file

- [ ] **No package dependency conflicts**
  - No version conflicts in dependency graph
  - All package references resolve correctly

- [ ] **No security vulnerabilities**
  - Security scan clean (already verified in assessment)
  - No new vulnerabilities introduced

#### Quality Criteria

- [ ] **Code quality maintained**
  - Code analysis passes
  - No degradation in code metrics
  - Best practices followed for .NET 10

- [ ] **Documentation updated**
  - README reflects .NET 10 requirement
  - Solution file reference updated to .slnx
  - Breaking changes documented

- [ ] **CI/CD pipeline updated**
  - Build scripts reference .slnx
  - Pipeline uses .NET 10 SDK
  - All pipeline stages succeed

#### Functional Criteria

- [ ] **Core functionality validated**
  - Workflow execution works
  - XAML processing functional
  - Runtime behavior correct

- [ ] **Performance acceptable**
  - Benchmarks complete successfully
  - Performance within acceptable range of .NET 6
  - No significant performance regressions

- [ ] **Development environment ready**
  - Solution opens in Visual Studio 2022
  - IntelliSense works correctly
  - Debugging functional

---

## Timeline and Effort Estimates

### Phase-by-Phase Timeline

| Phase | Projects | Estimated Time | Cumulative Time |
|-------|----------|---------------|----------------|
| **Phase 1** | 1 project (Foundation) | 2-3 hours | 3 hours |
| **Phase 2** | 2 projects (Core Runtime) | 4-5 hours | 8 hours |
| **Phase 3** | 7 projects (Extended + Tests) | 14-20 hours | 28 hours |
| - 3A: Quorum.CoreWF.Core | (Critical - 8-12 hours) | | |
| - 3B-3G: Other projects | (6-8 hours total) | | |
| **Phase 4** | 6 projects (Advanced Tests) | 10-14 hours | 42 hours |
| **Phase 5** | 1 project (Console App) | 1 hour | 43 hours |
| **Phase 6** | Solution Migration | 2-3 hours | 46 hours |
| **Final Testing** | Full validation | 4-6 hours | 52 hours |

**Total Estimated Time**: 46-52 hours

**Recommended Schedule**: 2-3 weeks with dedicated focus, allowing time for:
- Testing and validation
- Issue investigation
- Code review
- Documentation updates
- Buffer for unexpected issues

---

## Key Reminders

### Migration Principles

1. **Respect Dependency Order**: Never migrate a project before its dependencies
2. **Test Incrementally**: Validate at project, phase, and solution levels
3. **Commit Frequently**: Each project or sub-phase gets a dedicated commit
4. **Document Changes**: Note any API changes, behavioral differences, or workarounds
5. **Focus on Quorum.CoreWF.Core**: This project needs extra attention (2,123 issues)

### Critical Success Factors

- **CodeDom Migration**: Primary challenge in Quorum.CoreWF.Core
- **Behavioral Testing**: Required for 5 projects with behavioral changes
- **Deprecated Packages**: Plan follow-up work for Azure package migrations
- **Solution Format**: Validate .slnx works in all development environments

### If You Get Stuck

- Review .NET 10 breaking changes documentation
- Check project-specific API compatibility issues in assessment.md
- Consult .NET upgrade guides and community resources
- Consider seeking peer review for complex CodeDom changes
- Rollback to last stable commit and reassess approach

---

## Next Steps

1. **Review This Plan**: Ensure understanding of all phases and steps
2. **Confirm Prerequisites**: Verify .NET 10 SDK installed
3. **Backup Repository**: Create backup branch or tag before starting
4. **Begin Phase 1**: Start with Quorum.CoreWF.Xaml migration
5. **Follow Testing Protocol**: Validate at each level
6. **Track Progress**: Check off items as completed
7. **Document Issues**: Note any unexpected challenges or solutions

---

## Conclusion

This migration plan provides a structured, dependency-driven approach to upgrading the Quorum.CoreWF solution from .NET 6 to .NET 10 and modernizing the solution file format. The incremental strategy reduces risk by validating each dependency level before proceeding, with special attention given to the high-complexity Quorum.CoreWF.Core project.

The plan is designed to be **actionable** (step-by-step instructions), **specific** (exact file paths and commands), **complete** (covers all 17 projects), and **realistic** (includes time estimates and risk management).

**Total Scope:**
- 17 projects migrated
- 11 packages updated
- 3 deprecated packages documented
- 2,487 issues addressed
- Solution format modernized

**Expected Outcome:**
A fully functional .NET 10 solution with modern .slnx format, clean builds, passing tests, and maintained code quality.

---

*This plan is ready for execution. Would you like to proceed to the Execution stage?*
