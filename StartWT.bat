wt -M -d "c:\dev\eventstore-bank-demo\src\AccountsApp"; ^
split-pane -V -d "c:\dev\eventstore-bank-demo\src\SanctionsApp"; ^
move-focus left; ^
split-pane -H -d "c:\dev\eventstore-bank-demo\src\PaymentSchemeSimulatorApp"; ^
move-focus right; ^
split-pane -H -d "c:\dev\eventstore-bank-demo\src\PaymentSchemeDomain"
 