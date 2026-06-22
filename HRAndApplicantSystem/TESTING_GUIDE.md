# HR Applicant System - Testing Guide

## System Overview

The HR Applicant System is a complete application workflow management system with:
- **Applicant Portal**: View applications, manage profile, view interviews, check documents
- **HR Dashboard**: Screen applications, schedule interviews, view reports
- **HR Manager Dashboard**: Make final hiring decisions, access comprehensive reports
- **Admin Dashboard**: Full system management, audit trails, requirement management

---

## Role-Based Access Control

### Roles Available
1. **Applicant** (RoleID: 1) - Job seekers
2. **HR Staff** (RoleID: 2) - Can screen and schedule interviews
3. **HR Manager** (RoleID: 3) - Can make final decisions, view full reports
4. **Admin** (RoleID: 4) - Full system access

### Permission Matrix

| Action | Applicant | HR Staff | HR Manager | Admin |
|--------|-----------|----------|------------|-------|
| View own profile | ✓ | | | |
| Edit own profile | ✓ | | | |
| Browse jobs | ✓ | | | |
| Apply for jobs | ✓ | | | |
| View own applications | ✓ | | | |
| Submit documents | ✓ | | | |
| View own interviews | ✓ | | | |
| Screen applications | | ✓ | ✓ | ✓ |
| Schedule interviews | | ✓ | ✓ | ✓ |
| Evaluate interviews | | ✓ | ✓ | ✓ |
| Make hiring decisions | | | ✓ | ✓ |
| View all applications | | ✓ | ✓ | ✓ |
| View full reports | | | ✓ | ✓ |
| Manage users | | | | ✓ |
| Manage requirements | | | | ✓ |
| View audit trails | | | | ✓ |

---

## Feature Testing Checklist

### 1. Applicant Dashboard Features

#### Dashboard Summary
- [ ] View total applications count
- [ ] See status breakdown by category
- [ ] View recent applications (last 3)
- [ ] See next steps and recommendations
- [ ] Access quick actions menu

#### Interview Schedule
- [ ] View all scheduled interviews
- [ ] See interview date, time, and location
- [ ] View associated job titles
- [ ] Display total count of interviews

#### Application Status Timeline
- [ ] Select application to view timeline
- [ ] Display chronological status history
- [ ] Show remarks/notes at each status change
- [ ] Display date/time for each status change
- [ ] Show who made each status change

---

### 2. Applicant Profile Management

#### Profile Viewing
- [ ] Display first name, last name
- [ ] Display contact number
- [ ] Display address
- [ ] Display education background
- [ ] Display skills
- [ ] Show username

#### Profile Editing
- [ ] Edit first name with default value shown
- [ ] Edit last name with default value shown
- [ ] Edit contact number with validation
- [ ] Edit address
- [ ] Edit education
- [ ] Edit skills
- [ ] Save changes successfully
- [ ] Show confirmation message on save
- [ ] Refresh applicant data after edit

---

### 3. Application Workflow

#### Create Application
- [ ] Browse available jobs (Active/Open status only)
- [ ] Click "Apply Now" for a job
- [ ] Create draft application
- [ ] See application in "Draft" status
- [ ] Cannot submit duplicate applications for same job

#### Submit Application
- [ ] Submit from Draft status
- [ ] Application moves to "Submitted" status
- [ ] Application becomes locked (read-only)
- [ ] Cannot edit submitted application
- [ ] Status history records submission

#### Application Locked After Submission
- [ ] Draft applications are editable
- [ ] Submitted applications are locked
- [ ] Cannot re-apply to same job after submission
- [ ] Can apply to different jobs

---

### 4. Document Management

#### View Pending Documents
- [ ] See all required documents
- [ ] Display document types
- [ ] Show submission status
- [ ] Display remarks/descriptions

#### Submit Documents
- [ ] Select document type
- [ ] Add remarks/description
- [ ] Save document submission
- [ ] Document marked as "Submitted"
- [ ] Can update document after initial submission

#### Document Status Display
- [ ] Required Documents section visible
- [ ] Shows "Not Submitted" for missing docs
- [ ] Shows "Submitted" for uploaded docs
- [ ] Shows remarks if provided

---

### 5. HR Staff - Application Screening

#### Screen Application
- [ ] View submitted applications
- [ ] Display applicant details
- [ ] Show applicant education and skills
- [ ] Display submitted documents
- [ ] Show job description

#### Screening Decision
- [ ] Mark as "Qualified" (Shortlisted)
- [ ] Mark as "Not Qualified" (Rejected)
- [ ] Add remarks/feedback
- [ ] Application status updates
- [ ] Status history records screening result
- [ ] Applicant notified of status change

---

### 6. Interview Management

#### Schedule Interview
- [ ] Show shortlisted applicants
- [ ] Select applicant to schedule
- [ ] Enter interview date (validates future date)
- [ ] Enter interview time (HH:MM format)
- [ ] Select mode (Face-to-Face or Online)
- [ ] Enter location/meeting link
- [ ] Enter interviewer name
- [ ] Save interview schedule
- [ ] Application status changes to "Interview Scheduled"
- [ ] Status history records scheduling

#### Evaluate Interview
- [ ] Show applicants with scheduled interviews
- [ ] Select applicant to evaluate
- [ ] Enter interview score (0-100)
- [ ] Select result (Pass/Fail)
- [ ] Add remarks/feedback
- [ ] Select recommendation (For Final Review or Reject)
- [ ] Save evaluation
- [ ] Application status updates accordingly
- [ ] Status history records evaluation

#### Cancel Interview
- [ ] Show scheduled interviews
- [ ] Select interview to cancel
- [ ] Confirm cancellation
- [ ] Add reason for cancellation
- [ ] Interview schedule deleted
- [ ] Application status reverted

#### Reschedule Interview
- [ ] Show scheduled interviews
- [ ] Select interview to reschedule
- [ ] Display current date/time
- [ ] Enter new date (validates future)
- [ ] Enter new time
- [ ] Enter new location
- [ ] Save rescheduled interview
- [ ] Status history records change

---

### 7. HR Manager - Hiring Decisions

#### Make Final Decision
- [ ] Show applicants "For Final Review"
- [ ] Display applicant details
- [ ] Display application history
- [ ] Select decision (Accept/Reject/On Hold)
- [ ] Add remarks
- [ ] Save decision
- [ ] Application status changes to "Accepted"/"Rejected"/"On Hold"
- [ ] Status history records decision

---

### 8. Reports & Statistics

#### Application Metrics
- [ ] Display total applications count
- [ ] Show status breakdown (all statuses)
- [ ] Calculate percentages for each status
- [ ] Format data in readable table

#### Interview Metrics
- [ ] Display total interviews evaluated
- [ ] Show pass/fail counts
- [ ] Calculate pass rate percentage
- [ ] Calculate fail rate percentage
- [ ] Display detailed result breakdown

#### Time-to-Hire Metrics
- [ ] Show total applications tracked
- [ ] Display average days to hire
- [ ] Show minimum days to hire
- [ ] Show maximum days to hire
- [ ] Display median days to hire

#### Hiring Decision Metrics
- [ ] Show total hiring decisions
- [ ] Display offer/hired count
- [ ] Display rejection count
- [ ] Calculate offer rate percentage
- [ ] Calculate rejection rate percentage

#### Complete Reports Summary
- [ ] Combine all metrics
- [ ] Display executive summary
- [ ] Show key statistics side-by-side

---

### 9. Status History Tracking

#### Status Timeline Display
- [ ] Show all status changes in chronological order
- [ ] Display date/time for each change
- [ ] Show new status
- [ ] Display remarks at each change
- [ ] Show who made the change (changed_by)

#### Statuses in Workflow
- [ ] Draft - Initial state
- [ ] Submitted - After submission
- [ ] Under Review - HR staff begins review
- [ ] Shortlisted - After screening (Qualified)
- [ ] Interview Scheduled - After scheduling
- [ ] For Final Review - After interview eval (Pass)
- [ ] Accepted - Final decision
- [ ] Rejected - Final decision

---

### 10. Document Requirement Management

#### View Requirements
- [ ] Display all requirement types
- [ ] Show requirement names
- [ ] Format in readable table

#### Add Requirement
- [ ] Enter requirement name
- [ ] Validate input not empty
- [ ] Save new requirement
- [ ] Confirm success message

#### Remove Requirement
- [ ] Select requirement to remove
- [ ] Confirm removal
- [ ] Requirement deleted
- [ ] Confirm deletion message

---

## Database Validation Tests

### Test Database Schema

Run these SQL queries to verify data integrity:

```sql
-- Check application status values
SELECT DISTINCT [Status] FROM [Applications];

-- Verify application status history
SELECT [ApplicationID], [Status], [DateChanged], [ChangedBy] 
FROM [ApplicationStatusHistory] 
ORDER BY [ApplicationID], [DateChanged] DESC;

-- Check interview schedules
SELECT [ApplicationID], [InterviewDate], [InterviewTime], [Status]
FROM [InterviewSchedules];

-- Verify applicant documents
SELECT [ApplicantID], [JobID], [RequirementTypeID], [DocumentStatus]
FROM [ApplicantDocuments];

-- Check hiring decisions
SELECT [ApplicationID], [Decision], [DecisionDate]
FROM [HiringDecisions];
```

---

## Test Scenarios

### Scenario 1: Complete Application Workflow
1. Applicant logs in
2. Browses available jobs
3. Applies for a job
4. Submits documents
5. HR screens application
6. HR schedules interview
7. HR evaluates interview
8. HR Manager makes decision
9. Applicant views final status

**Expected Result**: All status transitions recorded in history

### Scenario 2: Rejected Application
1. HR screens application
2. Marks as "Not Qualified"
3. Application rejected
4. Applicant cannot reapply to same job
5. Can apply to different jobs

**Expected Result**: Application locked, cannot modify

### Scenario 3: Multiple Applications
1. Applicant applies to 3 different jobs
2. Different statuses for each
3. View dashboard shows all 3
4. View timeline for each

**Expected Result**: Dashboard correctly aggregates all applications

### Scenario 4: Interview Rescheduling
1. Interview scheduled
2. Reschedule to new date/time
3. Old schedule removed
4. New schedule created
5. Status history updated

**Expected Result**: Only latest schedule shown

### Scenario 5: Role-Based Access
1. Applicant tries to access HR functions
2. System denies access
3. Shows permission denied message
4. HR staff can access HR functions

**Expected Result**: Permission checks working correctly

---

## Performance Tests

### Load Testing
- [ ] System handles 100 applications
- [ ] Dashboard loads in < 2 seconds
- [ ] Reports generate in < 5 seconds
- [ ] Search functionality responsive

### Stress Testing
- [ ] Multiple users accessing simultaneously
- [ ] Database connections properly managed
- [ ] No timeout errors
- [ ] Data consistency maintained

---

## Security Tests

### Data Validation
- [ ] SQL injection prevention (parameterized queries)
- [ ] Password hashing verification
- [ ] Input validation on all forms
- [ ] Date/time validation

### Access Control
- [ ] Unauthorized users cannot access functions
- [ ] Role-based permissions enforced
- [ ] Audit trail records all actions
- [ ] User cannot modify other users' data

---

## Error Handling Tests

### Invalid Input
- [ ] Empty fields properly validated
- [ ] Date validation works (past dates rejected)
- [ ] Time validation works (HH:MM format)
- [ ] Duplicate application prevention

### Database Errors
- [ ] Connection failures handled gracefully
- [ ] Error messages displayed to user
- [ ] Data not corrupted on errors
- [ ] Transactions properly rolled back

---

## Regression Testing

After any changes, verify:
- [ ] All existing features still work
- [ ] No new errors introduced
- [ ] Database integrity maintained
- [ ] Performance not degraded
- [ ] Status transitions still correct

---

## Documentation Strings

### Key Methods to Verify

```csharp
// Database Helper Methods
GetApplicationStatusHistory(int applicationID)
GetApplicantInterviews(int applicantID)
DisplayApplicationTimeline(int applicationID)
GetApplicationMetrics()
GetInterviewMetrics()
GetTimeToHireMetrics()
GetHiringDecisionMetrics()

// Service Methods
ApplicantDashboardService.ShowDashboard()
DashboardSummaryService.ShowInterviewSchedule()
DashboardSummaryService.ShowApplicationTimeline()
DocumentSubmissionService.ViewPendingDocuments()
InterviewService.ScheduleInterview()
InterviewService.EvaluateInterview()
HiringDecisionService.ReviewForFinalDecision()
ReportsService.ShowReportsMenu()

// Role Validation
RoleValidator.EnsurePermission()
RoleValidator.CanPerformAction()
```

---

## Known Limitations & Notes

1. **Draft Applications**: Not shown in any lists once submitted
2. **Document Status**: Only "Submitted" or "Missing" - no "Verified" feedback to applicants
3. **Email Notifications**: Simulated with console output (implement actual email later)
4. **Application Edit**: Cannot edit submitted applications
5. **Status Transitions**: Must follow the defined workflow

---

## Test Environment Setup

### Test Data Creation

```csharp
// Create test applicant
Username: testapplicant
Password: Test@123
Role: Applicant

// Create test HR Staff
Username: testhr
Password: Test@123
Role: HR Staff

// Create test HR Manager
Username: testmanager
Password: Test@123
Role: HR Manager

// Create test jobs
Job 1: Software Developer
Job 2: Business Analyst
Job 3: Project Manager

// Create test requirements
Resume
Transcript
Valid ID
```

---

## Sign-Off

- [ ] All tests passed
- [ ] No critical bugs found
- [ ] System ready for defense
- [ ] Documentation complete
- [ ] Performance acceptable

**Tested By**: ________________  
**Date**: ________________  
**Build Version**: 1.0.0
