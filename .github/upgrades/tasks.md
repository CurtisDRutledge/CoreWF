# .NET 10 Upgrade and Solution Migration - Execution Tasks

**Solution**: Quorum.CoreWF  
**Target Framework**: .NET 10.0  
**Projects**: 17  
**Strategy**: Incremental migration by dependency level  
**Branch**: develop (in-place upgrade)

---
**Progress**: 2/19 tasks complete (11%) ![11%](https://progress-bar.xyz/11)
## Task Execution Progress

### Phase 1: Foundation Layer

- [?] **TASK-001**: Upgrade Quorum.CoreWF.Xaml to .NET 10 *(Completed: 2026-02-08 15:41)*
  - [?] Update TargetFramework to net10.0 in project file
  - [?] Build project and resolve compilation errors
  - [?] Address API incompatibilities
  - [?] Eliminate build warnings
  - [?] Verify build succeeds
  - [?] Commit changes

---

### Phase 2: Core Runtime Libraries

- [?] **TASK-002**: Upgrade Microsoft.CodeAnalysis.VisualBasic.Scripting to .NET 10 *(Completed: 2026-02-08 15:45)*
  - [?] Update TargetFramework to net10.0 in project file
  - [?] Build project and resolve compilation errors
  - [?] Address VB.NET-specific API changes
  - [?] Eliminate build warnings
  - [?] Verify scripting functionality
  - [?] Commit changes

- [ ] **TASK-003**: Upgrade Quorum.CoreWF.Runtime to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities (25 issues)
  - [ ] Eliminate build warnings
  - [ ] Verify build succeeds
  - [ ] Run unit tests if available
  - [ ] Commit changes

---

### Phase 3: Extended Libraries and Test Infrastructure

- [ ] **TASK-004**: Upgrade Quorum.CoreWF.Core to .NET 10 (CRITICAL - High Complexity)
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update Microsoft.Extensions.Configuration to 10.0.2
  - [ ] Update Microsoft.Extensions.Configuration.Json to 10.0.2
  - [ ] Update Microsoft.Extensions.DependencyInjection to 10.0.2
  - [ ] Update Microsoft.Extensions.Logging to 10.0.2
  - [ ] Update Microsoft.Extensions.Options to 10.0.2
  - [ ] Update System.CodeDom to 10.0.2
  - [ ] Update System.Drawing.Common to 10.0.2
  - [ ] Update Newtonsoft.Json to 13.0.4
  - [ ] Build project and resolve compilation errors
  - [ ] Address CodeDom API changes (2,164 issues)
  - [ ] Address API incompatibilities
  - [ ] Address behavioral changes
  - [ ] Eliminate build warnings
  - [ ] Run unit tests
  - [ ] Verify CodeDom functionality
  - [ ] Commit changes

- [ ] **TASK-005**: Upgrade Quorum.CoreWF.EtwTracking to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Eliminate build warnings
  - [ ] Verify ETW tracking functionality
  - [ ] Commit changes

- [ ] **TASK-006**: Upgrade TestObjects to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities (94 issues)
  - [ ] Eliminate build warnings
  - [ ] Verify build succeeds
  - [ ] Commit changes

- [ ] **TASK-007**: Upgrade WorkflowApplicationTestExtensions to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Eliminate build warnings
  - [ ] Verify build succeeds
  - [ ] Commit changes

- [ ] **TASK-008**: Upgrade CustomTestObjects to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Eliminate build warnings
  - [ ] Verify build succeeds
  - [ ] Commit changes

- [ ] **TASK-009**: Upgrade ImperativeTestCases to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Eliminate build warnings
  - [ ] Run test suite
  - [ ] Verify all tests pass
  - [ ] Commit changes

- [ ] **TASK-010**: Upgrade System.Xaml.TestCases to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities and behavioral changes
  - [ ] Eliminate build warnings
  - [ ] Run test suite
  - [ ] Verify all tests pass
  - [ ] Commit changes

---

### Phase 4: Advanced Test Projects and Benchmarks

- [ ] **TASK-011**: Upgrade TestCases.Activities to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Address behavioral changes
  - [ ] Eliminate build warnings
  - [ ] Run test suite
  - [ ] Verify all tests pass
  - [ ] Commit changes

- [ ] **TASK-012**: Upgrade TestCases.Runtime to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities
  - [ ] Eliminate build warnings
  - [ ] Run test suite
  - [ ] Verify all tests pass
  - [ ] Commit changes

- [ ] **TASK-013**: Upgrade TestCases.Workflows to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities (56 issues)
  - [ ] Eliminate build warnings
  - [ ] Run test suite
  - [ ] Verify all tests pass
  - [ ] Validate workflow execution patterns
  - [ ] Commit changes

- [ ] **TASK-014**: Upgrade TestCases.Xaml to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities and behavioral changes
  - [ ] Eliminate build warnings
  - [ ] Run test suite
  - [ ] Verify all tests pass
  - [ ] Commit changes

- [ ] **TASK-015**: Upgrade CoreWf.Benchmarks to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Eliminate build warnings
  - [ ] Run sample benchmarks
  - [ ] Commit changes

- [ ] **TASK-016**: Upgrade Perf.AssemblyReference.Benchmarks to .NET 10
  - [ ] Update TargetFramework to net10.0;net10.0-windows in project file
  - [ ] Update package references if needed
  - [ ] Note deprecated packages for future migration
  - [ ] Build project and resolve compilation errors
  - [ ] Address API incompatibilities and behavioral changes
  - [ ] Eliminate build warnings
  - [ ] Run sample benchmarks
  - [ ] Commit changes

---

### Phase 5: Top-Level Application

- [ ] **TASK-017**: Upgrade TestConsole to .NET 10
  - [ ] Update TargetFramework to net10.0 in project file
  - [ ] Update package references if needed
  - [ ] Build project and resolve compilation errors
  - [ ] Eliminate build warnings
  - [ ] Run console application
  - [ ] Verify expected functionality
  - [ ] Commit changes

---

### Phase 6: Solution File Migration

- [ ] **TASK-018**: Migrate solution from .sln to .slnx format
  - [ ] Verify all 17 projects build successfully
  - [ ] Run full solution build
  - [ ] Create backup of Quorum.CoreWF.sln
  - [ ] Convert solution to .slnx format using Visual Studio 2022
  - [ ] Validate new .slnx file opens correctly
  - [ ] Verify all projects load in .slnx
  - [ ] Build solution using .slnx file
  - [ ] Update documentation references
  - [ ] Update CI/CD pipelines if needed
  - [ ] Delete old .sln file
  - [ ] Commit changes

---

### Final Validation

- [ ] **TASK-019**: Perform comprehensive solution validation
  - [ ] Build entire solution with dotnet build
  - [ ] Run all test projects
  - [ ] Execute CoreWf.Benchmarks
  - [ ] Execute Perf.AssemblyReference.Benchmarks
  - [ ] Run TestConsole application
  - [ ] Verify no package conflicts
  - [ ] Run security vulnerability scan
  - [ ] Verify code analysis clean
  - [ ] Update README.md with .NET 10 requirement
  - [ ] Document any breaking changes encountered
  - [ ] Final commit

---

## Execution Notes

### Task Dependencies
- Tasks must be executed in order within each phase
- Do not proceed to next phase until current phase is complete
- Each task should result in a git commit

### Critical Focus Areas
- **TASK-004** (Quorum.CoreWF.Core): Highest complexity with 2,123 issues
- **TASK-018** (Solution migration): Affects entire solution structure

### Deprecated Packages (Document for Future Work)
- Azure.Identity (1.14.0)
- Microsoft.Azure.ServiceBus (5.2.0)
- Microsoft.Azure.Storage.Blob (11.2.3)

### Rollback Points
- After each task completion (git commit)
- After each phase completion
- Before TASK-018 (pre-solution migration)

---

## Progress Tracking

**Total Tasks**: 19  
**Completed**: 0  
**In Progress**: 0  
**Remaining**: 19

**Estimated Total Time**: 46-52 hours  
**Target Completion**: 2-3 weeks

---

*This task list is generated from plan.md and represents the execution roadmap for the .NET 10 upgrade and solution migration.*
