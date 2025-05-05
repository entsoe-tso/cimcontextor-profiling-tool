# Security policy
## Overview
CimConteXtor contains executable code and external libraries, requiring security measures. We use static code scanning and dependency tracking to identify vulnerabilities, which are logged in Jira:
[CimConteXtor Jira Board](https://entsoe.atlassian.net/jira/software/c/projects/CIMC/issues?jql=project%20%3D%20%22CIMC%22%20ORDER%20BY%20created%20DESC)

## Reporting a vulnerability
If you identify any potential or confirmed security vulnerability in the CimConteXtor repository, please report it privately to the CIM Working Group (WG) maintainers via email at [cim@entsoe.eu](mailto:cim@entsoe.eu)

In your email:
- Provide your name, company, and contact information.
- Include detailed steps to reproduce the issue and describe its potential impact.

To assess the severity of the vulnerability, you may refer to the [Apache severity rating scale](https://security.apache.org/blog/severityrating/) for guidance.

## Response time
- You will receive an acknowledgment of your report within 5 working days.
- If the issue is validated as a security vulnerability, the repository users will be informed, and appropriate action will be taken:
    - Critical and important vulnerabilities will be resolved within 30 calendar days.
    - Moderate or low-severity issues will be addressed in the next planned release.

## When to report a vulnerability
Please report vulnerabilities in the following scenarios:
- When you believe the CimConteXtor repository may have been tampered with.
- When you suspect a security vulnerability but are unsure of its potential impact.

## Security Measures Implemented
To maintain the security and integrity of CimConteXtor, the following controls are in place:
1. Static Code Scanning & Dependency Tracking
* Code is continuously scanned for vulnerabilities using automated security tools.
* External dependencies are monitored and updated to address security risks.

2. Access Control
* Only authorized maintainers have write access.
* Multi-factor authentication (MFA) is enforced for all maintainers.

3. Change Validation & Vulnerability Management
* All pull requests must be reviewed and approved by at least one maintainer.
* Identified vulnerabilities are logged in Jira and assigned for resolution.

4. Audit & Monitoring
* GitHub audit logs are reviewed periodically to track changes and access.
* Alerts are configured for unauthorized access attempts or suspicious activity.

## Governance for Pull Requests

The ENTSO-E governance process ensures that repository integrity is maintained:
* Only maintainers may merge pull requests.
* Maintainers are experienced developers vetted by the ENTSO-E CIM Working Group.
* The approval process ensures that all changes are thoroughly reviewed for integrity and security before integration.

## Conclusion
CimConteXtor is classified as a Level 1 application, meaning no penetration testing or additional threat modelling is required. Instead, security is maintained through continuous monitoring, scanning, and governance controls to ensure the integrity and safety of the application.
