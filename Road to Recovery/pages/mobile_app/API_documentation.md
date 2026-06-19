# Road to Recovery — Mobile API Documentation

> **Base URL:** `https://<your-server>/WebService.asmx`  
> **Protocol:** SOAP / ASMX Web Service — `HTTP POST`, `Content-Type: application/json`  
> **Responses:** All endpoints return a JSON-serialised string.

---

## Overview

This API serves the **Road to Recovery** volunteer mobile app. It exposes four endpoints:

| Endpoint                        | Purpose                                                            |
| ------------------------------- | ------------------------------------------------------------------ |
| `LoginMobileApp`                | Authenticate a volunteer by phone number; returns full profile     |
| `GetVolunteerPreferencesMobile` | Get a volunteer's preferred ride days/shifts and geographic areas  |
| `GetMyUnityRidesMobile`         | Get all rides assigned to a specific volunteer (past & upcoming)   |
| `GetAllUnityRidesMobile`        | Get all open/unassigned rides, including patient medical equipment |

All endpoints use `POST`. On error, `LoginMobileApp` returns a structured `ResponseStatus` code; the other three return an empty array `[]` or empty object `{}`.

---

## 1. `LoginMobileApp`

Authenticates a volunteer by phone number and returns their full profile.

**Request:** `POST /WebService.asmx/LoginMobileApp`

```json
{ "userPhone": "0501234567" }
```

**Response fields** (`MobileLoginResponse`):

| Field                                    | Type      | Description                   |
| ---------------------------------------- | --------- | ----------------------------- |
| `ResponseStatus`                         | int       | `200` / `400` / `404` / `500` |
| `Message`                                | string    | Status message                |
| `Id`                                     | int?      | Volunteer DB ID               |
| `DisplayName`                            | string    | Full name (Hebrew)            |
| `FirstNameH` / `LastNameH`               | string    | Name parts (Hebrew)           |
| `CellPhone` / `CellPhone2` / `HomePhone` | string    | Phone numbers                 |
| `Address` / `Email`                      | string    | Contact details               |
| `JoinDate` / `BirthDate`                 | DateTime? | ISO 8601 dates                |
| `IsActive`                               | bool?     | Account active flag           |
| `Gender`                                 | string    | Hebrew gender string          |
| `AvailableSeats`                         | int?      | Seats in volunteer's car      |
| `JoinYear`                               | int?      | Year joined                   |
| `NoOfDocumentedRides` / `NoOfRides`      | int?      | Ride counts                   |
| `IsBooster` / `IsBabyChair`              | bool?     | Car seat equipment flags      |

**Status codes:**

| Code  | Meaning                         |
| ----- | ------------------------------- |
| `200` | Success — full profile returned |
| `400` | Missing/empty phone number      |
| `404` | Volunteer not found             |
| `500` | Server error or no DB response  |

---

## 2. `GetVolunteerPreferencesMobile`

Returns a volunteer's preferred ride days/shifts and geographic areas.

**Request:** `POST /WebService.asmx/GetVolunteerPreferencesMobile`

```json
{ "volunteerId": 42 }
```

**Response:**

```json
{
  "PreferredDays": [
    { "PreferedDayDayInWeek": "ראשון", "VolunteerId": 42, "Shift": "בוקר" }
  ],
  "PreferredAreas": [{ "PreferredArea": "מרכז", "VolunteerId": 42 }]
}
```

- `Shift` values: `"בוקר"` / `"אחהצ"` / `"כל"`
- Returns `{ "PreferredDays": [], "PreferredAreas": [] }` if `volunteerId <= 0` or on error.

---

## 3. `GetMyUnityRidesMobile`

Returns all Unity Rides assigned to a specific volunteer (past and upcoming).

**Request:** `POST /WebService.asmx/GetMyUnityRidesMobile`

```json
{ "volunteerId": 42 }
```

**Response fields** (array of `MobileMyUnityRide`):

| Field                    | Type      | Description                           |
| ------------------------ | --------- | ------------------------------------- |
| `RidePatNum`             | int?      | Unique ride ID                        |
| `PatientName`            | string    | Patient full name                     |
| `PatientCellPhone`       | string    | Patient phone                         |
| `Origin` / `Destination` | string    | Pickup / drop-off location            |
| `PickupTime`             | DateTime? | ISO 8601 date/time                    |
| `IsAfterNoon`            | bool?     | `true` = afternoon, `false` = morning |
| `Area`                   | string    | Geographic area (Hebrew)              |
| `Status`                 | string    | Ride status (Hebrew)                  |
| `AmountOfEscorts`        | int?      | Number of escorts                     |
| `IsInThePast`            | bool?     | `true` if ride already occurred       |

Returns `[]` if `volunteerId <= 0` or on error.

---

## 4. `GetAllUnityRidesMobile`

Returns all open/unassigned Unity Rides including each patient's required medical equipment.

**Request:** `POST /WebService.asmx/GetAllUnityRidesMobile`

```json
{}
```

**Response fields** (array of `MobileUnityRide`):

Same fields as `MobileMyUnityRide`, plus:

| Field               | Type                       | Description                               |
| ------------------- | -------------------------- | ----------------------------------------- |
| `PatientCellPhone2` | string                     | Secondary phone (nullable)                |
| `PatientHomePhone`  | string                     | Home phone (nullable)                     |
| `IsAnonymous`       | bool?                      | `true` = patient details hidden           |
| `PatientId`         | int?                       | Internal patient ID (`null` if anonymous) |
| `Equipments`        | `MobilePatientEquipment[]` | Required medical equipment                |

**`MobilePatientEquipment` fields:**

| Field         | Type   | Description             |
| ------------- | ------ | ----------------------- |
| `PatientId`   | int?   | Patient ID              |
| `EquipmentId` | int?   | Equipment type ID       |
| `Name`        | string | Equipment name (Hebrew) |

Returns `[]` on error.

---

## 5. Error Handling

| Scenario              | Behaviour                                             |
| --------------------- | ----------------------------------------------------- |
| Missing/invalid input | `ResponseStatus 400` with message                     |
| Resource not found    | `ResponseStatus 404` with message                     |
| Server exception      | `LoginMobileApp` → `500`; others → `[]` or `{}`       |
| DB returns no rows    | `LoginMobileApp` → `500, "No response from database"` |

**Stored Procedures:**

| Endpoint                        | Stored Procedure                                                                          |
| ------------------------------- | ----------------------------------------------------------------------------------------- |
| `LoginMobileApp`                | `dbo.USP_Login_with_phone_number_Mobile`                                                  |
| `GetVolunteerPreferencesMobile` | `dbo.USP_Get_Pref_Area_And_Time_Vol_Mobile`                                               |
| `GetMyUnityRidesMobile`         | `dbo.USP_Get_All_My_UnityRides_Mobile`                                                    |
| `GetAllUnityRidesMobile`        | `dbo.USP_Get_All_UnityRides_Mobile` + `dbo.USP_Get_Equipments_By_List_Patient_Ids_Mobile` |

---

_Last updated: 2024 — Road to Recovery Mobile Team_
