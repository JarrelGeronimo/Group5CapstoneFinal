# Job Vacancy Management System - Integration Guide

## 📋 Overview

The Job Vacancy Management system provides HR staff with comprehensive CRUD operations for managing job vacancies. This guide documents the complete integration, including database setup, architecture, and testing procedures.

---

## 🏗️ Architecture

### Service-Oriented Pattern
```
UI Layer (Forms)
    ↓
BusinessLogic Layer (JobVacancyManagementService)
    ↓
DataAccess Layer (DatabaseHelper)
    ↓
Database (Microsoft Access - HRApplicantData.accdb)
```

### Key Components

| Component | File | Purpose |
|-----------|------|---------|
| **Data Model** | `Models/JobVacancy.cs` | Job vacancy entity with properties |
| **Service** | `Services/JobVacancyManagementService.cs` | Business logic & validation |
| **Main Form** | `Forms/JobVacancyManagementForm.cs` | CRUD interface with filtering |
| **Create Dialog** | `Forms/CreateJobVacancyDialog.cs` | New vacancy creation with validation |
| **Edit Dialog** | `Forms/EditJobVacancyDialog.cs` | Edit existing vacancy with status control |
| **Dashboard Integration** | `Forms/MainForm.cs` | HR Dashboard navigation button |

---

## 📊 Database Schema

### JobVacancies Table

```sql
CREATE TABLE JobVacancies (
    JobID AutoNumber PRIMARY KEY,
    JobTitle Text(255) NOT NULL,
    JobDetail Memo NOT NULL,
    Status Text(20) NOT NULL,           -- "Open" or "Closed"
    DatePosted DateTime DEFAULT NOW()   -- Track creation date
);
```

### Required Migration

**Before running the application, execute this SQL in Microsoft Access:**

```sql
-- Add DatePosted column (if not already present)
ALTER TABLE JobVacancies 
ADD COLUMN DatePosted DateTime DEFAULT NOW();

-- Set DatePosted for existing records
UPDATE JobVacancies 
SET DatePosted = NOW() 
WHERE DatePosted IS NULL;
```

### Status Field Values

- **"Open"** - Vacancy actively accepts applications
- **"Closed"** - Vacancy no longer accepts applications (visible to HR, hidden from applicants)

---

## 🎯 Feature Breakdown

### 1. Job Vacancy Management Form (Main Interface)

**Location:** `Forms/JobVacancyManagementForm.cs`

**Features:**
- ✅ Display all job vacancies in DataGridView
- ✅ Filter by Status (All/Open/Closed)
- ✅ Search by Job Title (case-insensitive)
- ✅ Create new vacancies
- ✅ Edit existing vacancies
- ✅ Close vacancies (status change)
- ✅ Delete vacancies (with dependency checking)
- ✅ Real-time status display

**UI Layout:**
- Title: "Job Vacancy Management"
- Filter Panel: Status ComboBox + Search TextBox
- DataGridView: 5 columns (JobID, JobTitle, JobDetail, Status, DatePosted)
- Button Panel: Create, Edit, Close, Delete, Refresh, Exit

**Data Grid Columns:**
| Column | Width | Type | Editable |
|--------|-------|------|----------|
| JobID | 60px | Integer | No |
| JobTitle | 250px | String | No |
| JobDetail | 400px | String | No |
| Status | 100px | String | No |
| DatePosted | 150px | DateTime | No |

---

### 2. Create Job Vacancy Dialog

**Location:** `Forms/CreateJobVacancyDialog.cs`

**Form Size:** 500×400 (scrollable)

**Fields:**
1. **Job Title** (Required)
   - Type: TextBox (single-line)
   - Validation: Cannot be null/whitespace
   - Error Message: "Job title is required"

2. **Job Description** (Required)
   - Type: TextBox (multiline, 150px height)
   - Validation: Cannot be null/whitespace
   - Error Message: "Job description is required"

3. **Status** (Read-Only)
   - Display: "Open (new vacancies always start as Open)"
   - Auto-set: Status = "Open", DatePosted = DateTime.Now

**Action:**
- Click **Create** → Validates input → Calls `service.CreateJob()` → Logs audit trail → Closes dialog
- Click **Cancel** → Discards changes

**Audit Trail Entry:**
```
User: [HR Username]
Action: "Created job vacancy: [JobTitle]"
Timestamp: DateTime.Now
```

---

### 3. Edit Job Vacancy Dialog

**Location:** `Forms/EditJobVacancyDialog.cs`

**Form Size:** 500×450 (scrollable)

**Title:** "Edit Job Vacancy - ID: {JobID}"

**Fields:**
1. **Job Title** (Required)
   - Pre-populated with current value
   - Same validation as Create

2. **Job Description** (Required)
   - Pre-populated with current value
   - Same validation as Create

3. **Status** (Dropdown)
   - Options: "Open", "Closed"
   - Pre-selected to current status
   - Allows toggling between Open and Closed

4. **Date Posted** (Read-Only)
   - Display: "yyyy-MM-dd HH:mm" format
   - Shows when vacancy was created

**Action:**
- Click **Save** → Validates input → Calls `service.UpdateJob()` → Logs audit trail → Closes dialog
- Click **Cancel** → Discards changes

**Audit Trail Entry:**
```
User: [HR Username]
Action: "Updated job vacancy: [JobTitle] (ID: [JobID])"
Timestamp: DateTime.Now
```

---

### 4. HR Dashboard Integration

**Location:** `Forms/MainForm.cs` → `AddHRDashboardContent()` method

**New Button:**
- **Text:** "5. Manage Job Vacancies"
- **Position:** After "4. Make Hiring Decisions" button
- **Color:** Purple (RGB 153, 102, 204)
- **Size:** 300×100 pixels
- **Font:** Arial 11pt Bold
- **Click Handler:** Opens `JobVacancyManagementForm`

**Integration Code:**
```csharp
Button jobVacancyButton = new Button();
jobVacancyButton.Text = "5. Manage Job\nVacancies";
jobVacancyButton.Size = new System.Drawing.Size(300, 100);
jobVacancyButton.BackColor = System.Drawing.Color.FromArgb(153, 102, 204);
jobVacancyButton.ForeColor = System.Drawing.Color.White;
jobVacancyButton.Font = new Font("Arial", 11, FontStyle.Bold);
jobVacancyButton.Cursor = Cursors.Hand;
jobVacancyButton.Margin = new Padding(10);
jobVacancyButton.FlatStyle = FlatStyle.Flat;
jobVacancyButton.Click += (s, e) => OpenJobVacancyManagementForm();
contentPanel.Controls.Add(jobVacancyButton);
```

---

## 🔧 Service Methods

### JobVacancyManagementService Public API

#### Retrieve Operations
```csharp
// Get all vacancies (no filtering)
List<JobVacancy> GetAllJobs()

// Get only open vacancies (Status = "Open")
List<JobVacancy> GetOpenJobs()

// Get only closed vacancies (Status = "Closed")
List<JobVacancy> GetClosedJobs()

// Get single vacancy by ID
JobVacancy GetJobByID(int jobID)

// Check if job accepts applications
bool IsJobOpen(int jobID)
```

#### Create Operation
```csharp
// Create new job vacancy with validation and audit logging
// Returns: true if successful, false if validation fails
bool CreateJob(JobVacancy job, string hrUsername)

// Validation:
// - JobTitle cannot be null/whitespace
// - JobDetail cannot be null/whitespace
// - Automatically sets Status = "Open"
// - Automatically sets DatePosted = DateTime.Now

// Audit Trail:
// - Log Entry: "Created job vacancy: {JobTitle}"
```

#### Update Operation
```csharp
// Update existing job vacancy with validation and audit logging
// Returns: true if successful, false if validation fails
bool UpdateJob(JobVacancy job, string hrUsername)

// Validation:
// - JobTitle cannot be null/whitespace
// - JobDetail cannot be null/whitespace
// - Status must be "Open" or "Closed"

// Audit Trail:
// - Log Entry: "Updated job vacancy: {JobTitle} (ID: {JobID})"
```

#### Close Operation
```csharp
// Close a vacancy (set Status = "Closed") without deleting
// Returns: true if successful
bool CloseJob(int jobID, string hrUsername)

// Audit Trail:
// - Log Entry: "Closed job vacancy: {JobTitle} (ID: {JobID})"
```

#### Delete Operation
```csharp
// Delete a vacancy (with dependency checking)
// Returns: true if successful
// errorMessage: Populated with error reason if deletion fails
bool DeleteJob(int jobID, string hrUsername, out string errorMessage)

// Dependency Check:
// - Prevents deletion if vacancy has associated applications
// - Error Message: "Cannot delete this job vacancy because it has associated applications. Please close it instead."

// Audit Trail (if successful):
// - Log Entry: "Deleted job vacancy: {JobTitle} (ID: {JobID})"
```

---

## 📂 DatabaseHelper Methods

### Job Vacancy Operations

```csharp
// Core CRUD
List<JobVacancy> GetAllJobVacancies()
JobVacancy GetJobVacancyByID(int jobID)
bool CreateJobVacancy(JobVacancy job)
bool UpdateJobVacancy(JobVacancy job)
bool DeleteJobVacancy(int jobID)

// Filtering
List<JobVacancy> GetOpenJobVacancies()       // Status = "Open"
List<JobVacancy> GetClosedJobVacancies()     // Status = "Closed"

// Validation
bool JobHasApplications(int jobID)           // Check for linked Applications
bool CloseJobVacancy(int jobID)              // Set Status = "Closed"
```

---

## ✅ Testing Checklist

### Unit Tests

- [ ] **Create Vacancy**
  - [ ] Valid title and description → Creates successfully
  - [ ] Empty title → Shows error, doesn't create
  - [ ] Empty description → Shows error, doesn't create
  - [ ] New vacancy starts as "Open"
  - [ ] DatePosted set to current date/time

- [ ] **Edit Vacancy**
  - [ ] Change title → Updates successfully
  - [ ] Change description → Updates successfully
  - [ ] Toggle status Open→Closed → Updates successfully
  - [ ] Toggle status Closed→Open → Updates successfully
  - [ ] DatePosted remains unchanged

- [ ] **Close Vacancy**
  - [ ] Changes status to "Closed"
  - [ ] Removes from Open vacancies list
  - [ ] Still appears in Closed vacancies list

- [ ] **Delete Vacancy**
  - [ ] Vacancy with NO applications → Deletes successfully
  - [ ] Vacancy with applications → Shows error, doesn't delete
  - [ ] Error message is user-friendly

- [ ] **Filter/Search**
  - [ ] Status filter "All" → Shows all vacancies
  - [ ] Status filter "Open" → Shows only Open vacancies
  - [ ] Status filter "Closed" → Shows only Closed vacancies
  - [ ] Search "Software" → Shows matching titles (case-insensitive)
  - [ ] Search + Status filter → Combined filtering works

### Integration Tests

- [ ] **HR Dashboard Navigation**
  - [ ] "Manage Job Vacancies" button visible in HR Dashboard
  - [ ] Clicking button opens JobVacancyManagementForm
  - [ ] Form displays existing vacancies from database

- [ ] **Applicant Job Board**
  - [ ] Only "Open" vacancies appear to applicants
  - [ ] "Closed" vacancies hidden from applicant view
  - [ ] Applicants cannot apply to closed vacancies

- [ ] **Application Workflow**
  - [ ] Create job vacancy → Applicants see it → Apply
  - [ ] Close vacancy → Applicants cannot apply
  - [ ] Cannot delete vacancy with applications

- [ ] **Audit Trail**
  - [ ] Create vacancy logged
  - [ ] Update vacancy logged
  - [ ] Close vacancy logged
  - [ ] Delete vacancy logged (if successful)

---

## 🐛 Troubleshooting

### "DatePosted column does not exist" Error

**Problem:** The JobVacancies table lacks the DatePosted field.

**Solution:** 
1. Open HRApplicantData.accdb in Microsoft Access
2. Execute the migration SQL:
   ```sql
   ALTER TABLE JobVacancies ADD COLUMN DatePosted DateTime DEFAULT NOW();
   UPDATE JobVacancies SET DatePosted = NOW() WHERE DatePosted IS NULL;
   ```
3. Rebuild and re-run application

### "Cannot delete this job vacancy because it has associated applications"

**Problem:** Attempting to delete a vacancy that has applications.

**Solution:**
- Close the vacancy instead of deleting it (Status = "Closed")
- Or delete all associated applications first (not recommended)

### Form Won't Open

**Problem:** MainForm doesn't show the "Manage Job Vacancies" button.

**Solution:**
1. Verify `OpenJobVacancyManagementForm()` method exists in MainForm.cs
2. Check that button Click handler calls this method
3. Verify JobVacancyManagementForm.cs compiles without errors
4. Rebuild solution: `dotnet build`

---

## 📚 Related Documentation

- [JobVacancy Model](Models/JobVacancy.cs)
- [JobVacancyManagementService](Services/JobVacancyManagementService.cs)
- [DatabaseHelper Job Methods](Database/DatabaseHelper.cs)
- [Application Workflow](Models/Application.cs)

---

## 🔐 Security Notes

✅ **SQL Injection Prevention:** All queries use parameterized statements  
✅ **Audit Logging:** All CRUD operations logged with user info  
✅ **Dependency Validation:** Prevents orphaned applications  
✅ **Role-Based Access:** HR Dashboard restricted to HR staff (RoleID 2, 3, 4)  

---

## 📈 Future Enhancements

- [ ] Job vacancy templates (copy existing vacancy)
- [ ] Bulk operations (close/reopen multiple at once)
- [ ] Vacancy expiration dates (auto-close after X days)
- [ ] Application count display in vacancy list
- [ ] Vacancy statistics dashboard
- [ ] Email notifications when vacancy status changes

---

**Last Updated:** 2024  
**Status:** ✅ Complete and Integrated
