# LIFX.API Test Coverage Master Plan

## Executive Summary

**Current Coverage:** ~55-60% (Cloud API + LAN validation)  
**Target Coverage:** 95%+  
**Phase 1:** ? **COMPLETE** - LAN Protocol Tests  
**Total New Tests Required:** ~95 tests across 6 phases  
**Estimated Timeline:** 2-3 weeks  
**Progress:** Phase 1 of 6 complete (16% done)

---

## Current State Analysis

### ? What's Covered

#### Cloud HTTP API Tests (~85% coverage) - 56 tests
1. **LightsTests.cs** - 23 tests
   - List operations (all, by ID, by label)
   - Groups and locations listing
   - Power operations (on/off/toggle)
   - Color operations (named, RGB, HSBK, Kelvin)
   - Group operations
   - Advanced operations (SetStates, StateDelta)
   - Color validation

2. **EffectsTests.cs** - 14 tests
   - Single light effects (Breathe, Pulse, Morph, Flame)
   - Group effects (Clouds, Move)
   - Environment effects (Sunrise, Sunset)
   - Effect control (EffectsOff)
   - Clean cycle

3. **ScenesTests.cs** - 3 tests
   - List scenes
   - Activate scene (normal & fast)

4. **Unit Tests** - 16 tests
   - LifxColorTests (6 tests)
   - SelectorTests
   - PowerStateTests
   - DeserializationTests (4 tests)

#### LAN Protocol Tests ? **NEW** - 42 tests
5. **LanMessageTests.cs** - 12 tests
   - Frame header initialization and validation
   - Message parsing (valid/invalid packets)
   - Packet serialization
   - Message type enumeration
   - Binary payload handling

6. **LanDiscoveryTests.cs** - 10 tests
   - Client initialization (enabled/disabled)
   - Shared fixture validation
   - Device model structure
   - MAC address formatting

7. **LanDeviceTests.cs** - 10 tests
   - Device operation parameter validation
   - Null argument checks
   - PowerState enum validation

8. **LanLightTests.cs** - 10 tests
   - Light operation parameter validation
   - Kelvin range validation (2500-9000)
   - Transition duration validation
   - Color structure validation

**Total Current Tests:** **98 tests** (56 Cloud + 42 LAN)

---

## Phase Implementation Plan

### Phase 1: LAN Protocol Tests ? **COMPLETE**
**Target:** 45-50% ? 55-60% coverage  
**New Tests:** 42 tests (actual)  
**Duration:** 1 week (actual)  
**Status:** ? **COMPLETE**

#### Files Created:
- ? `Lifx.Api.Test/LanTestCollection.cs` - Shared fixture with `IAsyncLifetime`
- ? `Lifx.Api.Test/Lan/LanDiscoveryTests.cs` - 10 tests
- ? `Lifx.Api.Test/Lan/LanMessageTests.cs` - 12 tests
- ? `Lifx.Api.Test/Lan/LanDeviceTests.cs` - 10 tests
- ? `Lifx.Api.Test/Lan/LanLightTests.cs` - 10 tests

#### Key Achievements:
- ? Solved UDP socket port binding issue with shared fixture pattern
- ? Implemented graceful degradation for CI environments
- ? Validated all LAN API parameter contracts
- ? Tested message parsing and serialization
- ? Validated Kelvin temperature ranges (2500-9000)
- ? Tested transition duration boundaries

#### Test Coverage by Category:
- **Message Parsing**: 12 tests (ParseMessage, WritePacket, FrameHeader)
- **Discovery & Init**: 10 tests (Client setup, device models)
- **Device Operations**: 10 tests (Power, label, version, firmware)
- **Light Operations**: 10 tests (Color, state, infrared, validation)

#### Technical Decisions:
1. **Shared Test Fixture** - Single UDP socket for all tests (prevents conflicts)
2. **Skip Strategy** - Tests gracefully skip if socket unavailable (CI-friendly)
3. **Reflection Testing** - Used for private methods (ParseMessage, WritePacket)
4. **Parameter Validation Focus** - Emphasis on contract testing vs integration

---

### Phase 2: Utilities & Helpers Tests
**Target:** 55-60% ? 65-70% coverage  
**New Tests:** ~10 tests  
**File to Create:** `Lifx.Api.Test/Unit/UtilitiesTests.cs`  
**Status:** ? PENDING

#### Test Breakdown:
- [ ] RgbToHsl_Should_Convert_Red_Correctly
- [ ] RgbToHsl_Should_Convert_Green_Correctly
- [ ] RgbToHsl_Should_Convert_Blue_Correctly
- [ ] RgbToHsl_Should_Handle_White
- [ ] RgbToHsl_Should_Handle_Black
- [ ] RgbToHsl_Should_Handle_Gray_Scales
- [ ] Epoch_Should_Be_Unix_Epoch_1970
- [ ] Epoch_Time_Conversions_Should_Be_Accurate
- [ ] Utilities_Should_Handle_Boundary_Values
- [ ] Utilities_Should_Handle_Edge_Cases

---

### Phase 3: Extension Methods Tests
**Target:** 65-70% ? 75-80% coverage  
**New Tests:** ~15 tests  
**File to Create:** `Lifx.Api.Test/Unit/ExtensionsTests.cs`  
**Status:** ? PENDING

#### Test Breakdown:

##### LifxLightsApiExtensions (~6 tests)
- [ ] ListAsync_Should_Format_Selector_Correctly
- [ ] SetStateAsync_Should_Build_Request_Correctly
- [ ] TogglePowerAsync_Should_Build_Request
- [ ] SetStatesAsync_Should_Handle_Multiple_States
- [ ] StateDeltaAsync_Should_Build_Delta_Request
- [ ] CleanAsync_Should_Build_Clean_Request

##### LifxEffectsApiExtensions (~6 tests)
- [ ] BreatheAsync_Should_Build_Correct_Request
- [ ] PulseAsync_Should_Build_Correct_Request
- [ ] MorphAsync_Should_Build_Correct_Request
- [ ] FlameAsync_Should_Build_Correct_Request
- [ ] OffAsync_Should_Build_Correct_Request
- [ ] Effects_Should_Handle_Optional_Parameters

##### GroupExtensions (~3 tests)
- [ ] AsSelector_Should_Create_GroupId_Selector
- [ ] AsSelector_Should_Create_GroupLabel_Selector
- [ ] AsSelector_Should_Handle_Null_Group

---

### Phase 4: Model Validation & Serialization Tests
**Target:** 75-80% ? 85% coverage  
**New Tests:** ~12 tests  
**File to Enhance:** `Lifx.Api.Test/DeserializationTests.cs`  
**Status:** ? PENDING

#### Test Breakdown:
- [ ] Hsbk_ToString_Should_Format_All_Components
- [ ] Hsbk_ToString_Should_Handle_Null_Hue
- [ ] Hsbk_ToString_Should_Handle_Null_Saturation
- [ ] Hsbk_ToString_Should_Handle_Null_Brightness
- [ ] Hsbk_ToString_Should_Handle_Null_Kelvin
- [ ] Hsbk_ToString_Should_Clamp_Values_To_Range
- [ ] Selector_Explicit_Operator_Should_Parse_LightId
- [ ] Selector_Explicit_Operator_Should_Parse_GroupId
- [ ] Selector_Explicit_Operator_Should_Parse_LocationId
- [ ] Selector_ToString_Should_Format_Correctly
- [ ] PowerState_Should_Serialize_As_Lowercase
- [ ] ColorResult_Should_Handle_All_Null_Values

---

### Phase 5: Error Handling & Edge Cases
**Target:** 85% ? 92% coverage  
**New Tests:** ~18 tests  
**File to Create:** `Lifx.Api.Test/ErrorHandlingTests.cs`  
**Status:** ? PENDING

#### Test Breakdown:

##### Cloud API Errors (~8 tests)
- [ ] API_Should_Handle_401_Unauthorized
- [ ] API_Should_Handle_404_NotFound
- [ ] API_Should_Handle_429_RateLimiting
- [ ] API_Should_Handle_500_ServerError
- [ ] API_Should_Handle_Network_Timeout
- [ ] API_Should_Handle_Invalid_JSON_Response
- [ ] API_Should_Handle_Empty_Response
- [ ] API_Should_Validate_Required_Fields

##### LAN Protocol Errors (~6 tests)
- [ ] LAN_Should_Throw_When_Not_Enabled
- [ ] LAN_Should_Handle_Socket_Errors
- [ ] LAN_Should_Handle_Malformed_Packets
- [ ] LAN_Should_Timeout_On_No_Response
- [ ] LAN_Should_Handle_Discovery_Timeout
- [ ] LAN_Should_Handle_Concurrent_Messages

##### Validation Errors (~4 tests)
- [ ] SetColor_Should_Validate_Kelvin_Min
- [ ] SetColor_Should_Validate_Kelvin_Max
- [ ] SetColor_Should_Validate_Transition_Duration_Positive
- [ ] SetColor_Should_Validate_RGB_Range

---

### Phase 6: Integration & Scenario Tests
**Target:** 92% ? 95%+ coverage  
**New Tests:** ~10 tests  
**File to Create:** `Lifx.Api.Test/Integration/IntegrationTests.cs`  
**Status:** ? PENDING

#### Test Breakdown:
- [ ] Complete_Workflow_Discovery_To_Control
- [ ] Cloud_And_LAN_Should_Target_Same_Light
- [ ] Multiple_Concurrent_Color_Changes
- [ ] Multiple_Concurrent_Power_Toggles
- [ ] Scene_Activation_Should_Affect_All_Lights
- [ ] Effect_Should_Be_Cancellable
- [ ] State_Restore_Should_Work_After_Changes
- [ ] Rapid_State_Changes_Should_Not_Fail
- [ ] Complex_Multi_Light_Scenario
- [ ] Performance_Under_Load

---

## Progress Tracking

| Phase | Tests | Status | Coverage Impact | Priority | Completion |
|-------|-------|--------|----------------|----------|------------|
| Current | 56 | ? DONE | 45-50% | - | 100% |
| **Phase 1** | **+42** | **? COMPLETE** | **? 55-60%** | **? CRITICAL** | **100%** |
| Phase 2 | +10 | ? PENDING | ? 65-70% | ?? MEDIUM | 0% |
| Phase 3 | +15 | ? PENDING | ? 75-80% | ?? MEDIUM | 0% |
| Phase 4 | +12 | ? PENDING | ? 85% | ?? MEDIUM | 0% |
| Phase 5 | +18 | ? PENDING | ? 92% | ?? HIGH | 0% |
| Phase 6 | +10 | ? PENDING | ? 95%+ | ?? LOW | 0% |
| **TOTAL** | **163** | **16% DONE** | **95%+** | - | **16%** |

### Progress Chart
```
Phase 1: ???????????????????? 100% ? COMPLETE
Phase 2: ????????????????????   0%
Phase 3: ????????????????????   0%
Phase 4: ????????????????????   0%
Phase 5: ????????????????????   0%
Phase 6: ????????????????????   0%
Overall: ????????????????????  16%
```

---

## Testing Infrastructure

### ? Completed Infrastructure (Phase 1)
- ? **Shared Test Fixture Pattern** - `LanTestFixture` with `IAsyncLifetime`
- ? **Single UDP Socket Management** - Prevents port binding conflicts
- ? **Graceful Degradation** - Tests skip if socket unavailable
- ? **Test Collection** - `[Collection("LAN Tests")]` for proper isolation
- ? **Reflection-based Testing** - For private method validation

### ? Pending Infrastructure
- ? VCR-style HTTP recording for Cloud API
- ? Mock UDP socket abstraction (for better CI/CD)
- ? Performance benchmarking framework
- ? Code coverage reporting setup

### Configuration
```json
{
  "AppToken": "from-user-secrets",
  "EnableLanTests": true,
  "UseMockLanDevices": true,
  "LanTestTimeout": 10000,
  "TestLightId": "optional-specific-light-id",
  "TestGroupId": "optional-specific-group-id"
}
```

### Test Organization Structure
```
Lifx.Api.Test/
??? Cloud/                    # Cloud API integration tests
?   ??? LightsTests.cs       ? 23 tests
?   ??? EffectsTests.cs      ? 14 tests
?   ??? ScenesTests.cs       ? 3 tests
??? Lan/                      # LAN protocol tests ? NEW
?   ??? LanDiscoveryTests.cs ? 10 tests
?   ??? LanMessageTests.cs   ? 12 tests
?   ??? LanDeviceTests.cs    ? 10 tests
?   ??? LanLightTests.cs     ? 10 tests
??? Unit/                     # Pure unit tests
?   ??? LifxColorTests.cs    ? 6 tests
?   ??? SelectorTests.cs     ? tests
?   ??? PowerStateTests.cs   ? tests
?   ??? UtilitiesTests.cs    ? Phase 2
?   ??? ExtensionsTests.cs   ? Phase 3
??? Integration/              # End-to-end scenarios
?   ??? IntegrationTests.cs  ? Phase 6
??? DeserializationTests.cs  ? 4 tests (enhance in Phase 4)
??? LanTestCollection.cs     ? Shared fixture
```

---

## Phase 1 Lessons Learned

### What Worked Well ?
1. **Shared Fixture Pattern** - Solved socket binding elegantly
2. **Skip Strategy** - Graceful degradation for CI
3. **Reflection Testing** - Validated private methods effectively
4. **Parameter Validation** - Caught contract violations early

### Challenges Faced ??
1. **Hot Reload Issues** - Structural changes require debugger stop
2. **Socket Cleanup** - Need delay for proper disposal
3. **Test Isolation** - Sequential execution in LAN collection

### Improvements for Next Phases ??
1. **Mock UDP Socket** - Better isolation for Phase 5
2. **Parallel Execution** - Where possible in other test collections
3. **Coverage Reporting** - Set up Coverlet integration
4. **CI Pipeline** - Configure GitHub Actions with secrets

---

## CI/CD Considerations

### Build Pipeline Strategy
1. **Unit Tests** - Run on every commit (no external dependencies) ?
2. **LAN Tests** - Run on every commit (shared fixture, no hardware) ?
3. **Cloud API Tests** - Run on PR (requires API token secret) ?
4. **Integration Tests** - Run nightly (full environment) ?
5. **Coverage Report** - Generate and publish to PR ?

### Code Coverage Tools
- **Coverlet** - Coverage collection ?
- **ReportGenerator** - HTML reports ?
- **Codecov/Coveralls** - PR integration ?
- **Target**: Minimum 85% coverage on new code

---

## Success Criteria

### Phase 1 Success Criteria ?
- ? All 42 LAN tests passing
- ? No reduction in existing test coverage
- ? Socket binding issue resolved
- ? Documentation updated (PHASE1_PROGRESS.md, LAN_TESTS_FIX.md)
- ? Graceful degradation for CI

### Overall Success Criteria (Pending)
- ? 95%+ code coverage achieved
- ? All critical paths tested
- ? Both LAN and Cloud APIs covered (partial)
- ? Error scenarios handled
- ? Performance acceptable (< 5min test suite)
- ? Tests are maintainable and documented

---

## Timeline

### Actual vs Estimated

| Phase | Estimated | Actual | Variance |
|-------|-----------|--------|----------|
| Phase 1 | 5-7 days | 1 week | On track ? |
| Phase 2 | 2-3 days | TBD | - |
| Phase 3 | 3-4 days | TBD | - |
| Phase 4 | 2-3 days | TBD | - |
| Phase 5 | 4-5 days | TBD | - |
| Phase 6 | 3-4 days | TBD | - |
| **Total** | **2-3 weeks** | **1 week done** | **On schedule** |

---

## Risks & Mitigations

| Risk | Impact | Status | Mitigation |
|------|--------|--------|------------|
| LAN hardware unavailable | High | ? MITIGATED | Shared fixture with skip strategy |
| Cloud API rate limits | Medium | ? PENDING | Implement request throttling (Phase 5) |
| Flaky network tests | Medium | ? PENDING | Add retry logic (Phase 5) |
| Test suite too slow | Low | ? MONITOR | Currently < 2min total |
| Breaking API changes | Low | ? MONITOR | Version pinning active |
| Socket binding conflicts | High | ? RESOLVED | Shared fixture pattern |

---

## Change Log

| Date | Phase | Status | Notes |
|------|-------|--------|-------|
| 2024-01-XX | Initial | Planning | Master plan created |
| 2024-01-XX | Phase 1 | Started | LAN protocol test implementation begun |
| 2024-01-XX | Phase 1 | Issues | Socket binding conflicts discovered |
| 2024-01-XX | Phase 1 | Resolved | Shared fixture pattern implemented |
| 2024-01-XX | **Phase 1** | **? COMPLETE** | **42 tests passing, socket issue solved** |

---

## Next Actions

### Immediate (Post-Phase 1)
1. ? Update MASTER_PLAN.md with Phase 1 completion
2. ? Run full test suite to confirm no regressions
3. ? Update PHASE1_PROGRESS.md with final metrics
4. ? Commit Phase 1 work to version control

### Phase 2 Preparation
1. ? Review `Lifx.Api\Lan\Utilities.cs` for test candidates
2. ? Create `Lifx.Api.Test/Unit/UtilitiesTests.cs`
3. ? Implement RGB to HSL conversion tests
4. ? Add epoch time conversion tests

---

**Last Updated:** 2024-01-XX (Phase 1 Complete)  
**Document Owner:** Development Team  
**Review Cycle:** After each phase completion  
**Current Phase:** Starting Phase 2
