

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:7000";

export const API_ENDPOINTS = {
    
    login: `${API_URL}/api/auth/login`,
    signup: `${API_URL}/api/auth/signup`,
    forgetPassword: `${API_URL}/api/auth/forgetPassword`,
    resetPassword: `${API_URL}/api/auth/resetPassword`,

    updateMe: `${API_URL}/api/users/updateMe`,

    createAccount: `${API_URL}/api/accounts/createAccount`,
    updateAccount: (AccountId) => `${API_URL}/api/accounts/updateAccount/${AccountId}`,
    deleteAccount: (AccountId) => `${API_URL}/api/accounts/deleteAccount/${AccountId}`,
    getAccount: `${API_URL}/api/accounts/getAccount`,
    getAllAccounts: `${API_URL}/api/accounts/getAllAccounts`,
    projectedbalanceView: `${API_URL}/api/accounts/projectedbalanceView`,

    createBudget: `${API_URL}/api/budgets/createBudget`,
    updateBudget: (BudgetId) => `${API_URL}/api/budgets/updateBudget/${BudgetId}`,
    deleteBudget: (BudgetId) => `${API_URL}/api/budgets/deleteBudget/${BudgetId}`,
    getAllBudgets: `${API_URL}/api/budgets/getAllBudgets`,

    createCategory: `${API_URL}/api/categories/createCategory`,
    updateCategory: (CategoryId) => `${API_URL}/api/categories/updateCategory/${CategoryId}`,
    deleteCategory: (CategoryId) => `${API_URL}/api/categories/deleteCategory/${CategoryId}`,
    getAllCategories: `${API_URL}/api/categories/getAllCategories`,

    uploadFile: `${API_URL}/api/csvFileUpload/uploadFile`,
    confirmCsvImport: `${API_URL}/api/csvFileUpload/confirmImport`,

    getAllNofications: `${API_URL}/api/nofications/getAllNofications`,
    deleteNofication: (NoficationId) => `${API_URL}/api/nofications/deleteNofication/${NoficationId}`,
    deleteAllNofications: `${API_URL}/api/nofications/deleteAllNofications`,
    markNofication: (NoficationId) => `${API_URL}/api/nofications/markNofication/${NoficationId}`,
    markAllAsReadNofication: `${API_URL}/api/nofications/markAllAsReadNofication`,
    markAllAsUnreadNofication: `${API_URL}/api/nofications/markAllAsUnreadNofication`,

    getCategoriesSpend: `${API_URL}/api/reports/getCategoriesSpend`,
    getBalanceHistory: `${API_URL}/api/reports/getBalanceHistory`,
    getIncomeExpense: `${API_URL}/api/reports/getIncomeExpense`,

    createRecurringRule: `${API_URL}/api/recurringRules/createRecurringRule`,
    updateRecurringRule: (AutoPaymentId) =>  `${API_URL}/api/recurringRules/updateRecurringRule/${AutoPaymentId}`,
    deleteRecurringRule: (AutoPaymentId) => `${API_URL}/api/recurringRules/deleteRecurringRule/${AutoPaymentId}`,
    getAllRecurringRules: `${API_URL}/api/recurringRules/getAllRecurringRules`,
    getRecurringRule: `${API_URL}/api/recurringRules/getRecurringRule`,

    getCategoriesSpend: `${API_URL}/api/reports/getCategoriesSpend`,
    getBalanceHistory: `${API_URL}/api/reports/getBalanceHistory`,
    getIncomeExpense: `${API_URL}/api/reports/getIncomeExpense`,

    createTransaction: `${API_URL}/api/transactions/createTransaction`,
    updateTransaction: (TransactionId) => `${API_URL}/api/transactions/updateTransaction/${TransactionId}`,
    deleteTransaction: (TransactionId) => `${API_URL}/api/transactions/deleteTransaction/${TransactionId}`,
    getTransaction: `${API_URL}/api/transactions/getTransaction`,
    getAllTransactions: `${API_URL}/api/transactions/getAllTransactions`,
    getAllAccTransactions: (TransactionId) => `${API_URL}/api/transactions/getAllAccTransactions/${TransactionId}`,

    createTransfer: `${API_URL}/api/transfers/createTransfer`,
    getTransfer: `${API_URL}/api/transfers/getTransfer`,
    getAllTransfers: `${API_URL}/api/transfers/getAllTransfers`,
    getAllAccTransfers: (AccountId) => `${API_URL}/api/transfers/getAllAccTransfers/${AccountId}`

}