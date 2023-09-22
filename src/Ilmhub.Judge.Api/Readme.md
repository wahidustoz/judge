## Judge API

#### Judge API endpoints
- `/languagees` GET -- returns configured languages id and name properties
- `/judge` POST -- judges given source code agains testcases provided or saved to server earlier
    - languageId: int (required)
    - source: string (required)
    - maxCpu: long - max cpu usage allowed
        - we should make this nullable and when this is not sent, we consider inifinite cpu
    - maxMemory: long - max ram allowed
        - same as cpu
    - testcases: [ { id: string, input: string, output: stirng } ] - testcases to check agains\
        - each or none of the test cases must have input
        - each test case must have output
        - each test case must have unique id
        - id should be alphanumberic
        - this should be null when testcaseId is sent
    - testcaseId: guid - you can save your test cases into server before hand so that you dont need to send testcases for each run
        - server returns guid id of test case
        - we should check that testcase folder with this id exists
        - this should be null when testcases property is sent
- `/testcases` POST - uploades test case to server
    - [ { input: string, output: string } ] --  cant be