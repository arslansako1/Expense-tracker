import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useNavigate } from "react-router-dom";
import { useRef, useState, useEffect } from "react";
import "/src/Css/Dashboard/ImportBankStatementPage/ImportBankStatement.css";

export default function ImportBankStatement(){
     const navigate = useNavigate();
    const fileInputRef = useRef(null);
    const [file, setFile] = useState(null);
    const [preview, setPreview] = useState(null);
    const [accounts, setAccounts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [accountId, setAccountId] = useState("");
    const [categoryId, setCategoryId] = useState("");

    const [uploading, setUploading] = useState(false);
    const [confirming, setConfirming] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(null);

    useEffect(() => {
        fetchAccounts();
        fetchCategories();
    }, []);

    const fetchAccounts = async () => {
        const token = localStorage.getItem("token");
        if (!token) return;

        try {
            const response = await fetch(API_ENDPOINTS.getAllAccounts, {
                headers: { Authorization: "Bearer " + token },
            });
            const data = await response.json();
            setAccounts(data);

        } catch (error) {
            setError(`Error fetching all accounts: ${error.message}`);
            
        }
    };

    const fetchCategories = async () => {
        const token = localStorage.getItem("token");
        if (!token) return;

        try {
            const response = await fetch(API_ENDPOINTS.getAllCategories, {
                headers: { Authorization: "Bearer " + token },
            });
            const data = await response.json();
            setCategories(data);
        } catch (error) {
            setError(`Error fetching all categories: ${error.message}`);
            
        }
    };

    const handleFileChange = (e) => {
        const selectedFile = e.target.files[0];
        if (selectedFile) {
            if (!selectedFile.name.endsWith(".csv")) {
                setError("Please select a CSV file");
                return;
            }
            setFile(selectedFile);
            setError(null);
            setPreview(null);
            setSuccess(null);
        }
    };

    const handleUpload = async (e) => {
        e.preventDefault();

        if (!file) {
            setError("Please select a file");
            return;
        }

        const token = localStorage.getItem("token");
        if (!token) {
            setError("You must be logged in");
            return;
        }

        setUploading(true);
        setError(null);
        setSuccess(null);

        const formData = new FormData();
        formData.append("file", file);

        try {
            const response = await fetch(API_ENDPOINTS.uploadFile, {
                method: "POST",
                headers: { Authorization: "Bearer " + token },
                body: formData,
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to upload file");
            }

            const data = await response.json();
            
            setPreview(data);

        } catch (error) {        
            setError(`Error uploading file: ${error.message}`);

        } finally {
            setUploading(false);
        }
    };

    const handleConfirmImport = async () => {
        if (!accountId) {
            setError("Please select an account");
            return;
        }

        if (!preview || preview.successCount === 0) {
            setError("No valid transactions to import");
            return;
        }

        const token = localStorage.getItem("token");
        if (!token) {
            setError("You must be logged in");
            return;
        }

        setConfirming(true);
        setError(null);
        setSuccess(null);

        try {
            const response = await fetch(API_ENDPOINTS.confirmCsvImport, {
                method: "POST",
                headers: {
                    Authorization: "Bearer " + token,
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    AccountId: parseInt(accountId),
                    CategoryId: categoryId ? parseInt(categoryId) : null,
                    Transactions: preview.validTransactions,
                }),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to import");
            }

            const data = await response.json();
            setSuccess({
                imported: data.importedCount,
                skipped: data.skippedCount,
                errors: data.errors,
            });

            setFile(null);
            setPreview(null);
            if (fileInputRef.current) fileInputRef.current.value = "";
        } catch (error) {
            
            setError(`Error importing file: ${error.message}`);
        } finally {
            setConfirming(false);
        }
    };

    const handleReset = () => {
        setFile(null);
        setPreview(null);
        setError(null);
        setSuccess(null);
        if (fileInputRef.current) fileInputRef.current.value = "";
    };




     
  function navigateHome(){
    navigate("/MainPage");
  }

  function navigateAccounts(){
    navigate("/Accounts");
  }

  function navigateCategories(){
    navigate("/Categories");
  }

  function navigateBudgets(){
    navigate("/Budgets");
  }

  function navigateTransactions(){
    navigate("/Transactions");
  }

  function navigateTransfers(){
    navigate("/Transfers");
  }

  function navigateAutoPayments(){
    navigate("/AutoPayments");
  }

  function navigateImportBankStatement(){
    navigate("/ImportBankStatement");
  }

   function navigateReports(){
    navigate("/Reports");
  }

  function navigateNofications(){
    navigate("/Nofications");
  }

  function navigateProfile(){
    navigate("/Profile");
  }

  function navigateLogout(e){
    e.preventDefault();
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    window.location.href = "/Login";
  }

    return(
      <>
       <div>
        <header className="mainPage-header">
          <button className="header-btn" onClick={navigateHome}>Home</button>
          <button className="header-btn" onClick={navigateAccounts}>Accounts</button>
          <button className="header-btn" onClick={navigateCategories}>Categories</button>
          <button className="header-btn" onClick={navigateBudgets}>Budgets</button>
          <button className="header-btn" onClick={navigateTransactions}>Transactions</button>
          <button className="header-btn" onClick={navigateTransfers}>Transfers</button>
          <button className="header-btn" onClick={navigateAutoPayments}>Auto payments</button>
          <button className="header-bank" onClick={navigateImportBankStatement}>Import bank statement</button>
          <button className="header-btn" onClick={navigateReports}>Reports</button>
          <button className="header-btn" onClick={navigateNofications}>Nofications</button>
          <button className="header-btn" onClick={navigateProfile}>Profile</button>
          <button className="header-btn" onClick={navigateLogout}>Logout</button>
       </header>            
            <h1 className="importBank-title">Import bank statement</h1>
        </div>
    
        <div className="import-page">

            <div className="import-container">
                <h1>Import Bank Statement</h1>
                <p className="import-subtitle">
                    Upload a CSV file to import your bank transactions
                </p>

                <div className="import-form-row">
                    <div className="import-form-group">
                        <label>Account:</label>
                        <select value={accountId} onChange={(e) => setAccountId(e.target.value)}>
                            <option value="">Select an account</option>
                            {accounts.map((a) => (
                                <option key={a.id} value={a.id}>
                                    {a.name} (${a.balance})
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="import-form-group">
                        <label>Category:</label>
                        <select value={categoryId} onChange={(e) => setCategoryId(e.target.value)}>
                            <option value="">No category</option>
                            {categories.map((c) => (
                                <option key={c.id} value={c.id}>
                                    {c.name} {c.icon}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                <div className="upload-area">
                    <input
                        ref={fileInputRef}
                        type="file"
                        accept=".csv"
                        onChange={handleFileChange}
                        id="csv-upload"
                        style={{ display: "none" }}
                    />
                    <label htmlFor="csv-upload" className="upload-label">
                        <span className="upload-icon">📁</span>
                        <span className="upload-text">
                            {file ? file.name : "Click to select CSV file"}
                        </span>
                        <span className="upload-hint">
                            Format: Date, Description, Amount
                        </span>
                    </label>
                </div>

                <div className="import-actions">
                    <button
                        onClick={handleUpload}
                        disabled={!file || uploading}
                        className="btn-upload"
                    >
                        {uploading ? "Uploading..." : "Upload & Preview"}
                    </button>
                    {(file || preview) && (
                        <button onClick={handleReset} className="btn-reset">
                            Reset
                        </button>
                    )}
                </div>

                {error && <div className="alert alert-error">{error}</div>}

                {success && (
                    <div className="alert alert-success">
                        <strong>Import Complete!</strong>
                        <p>Imported: {success.imported} transactions</p>
                        {success.skipped > 0 && <p>Skipped: {success.skipped} duplicates</p>}
                        {success.errors.length > 0 && (
                            <div>
                                <p>Issues:</p>
                                <ul>
                                    {success.errors.map((e, i) => (
                                        <li key={i}>{e}</li>
                                    ))}
                                </ul>
                            </div>
                        )}
                    </div>
                )}

                {preview && (
                    <div className="preview-section">
                        <h2>Preview Results</h2>

                        <div className="stats-grid">
                            <div className="stat-card total">
                                <div className="stat-value">{preview.totalRows}</div>
                                <div className="stat-label">Total Rows</div>
                            </div>
                            <div className="stat-card success">
                                <div className="stat-value">{preview.successCount}</div>
                                <div className="stat-label">✅ Valid</div>
                            </div>
                            <div className="stat-card error">
                                <div className="stat-value">{preview.errorCount}</div>
                                <div className="stat-label">❌ Errors</div>
                            </div>
                        </div>

                        {preview.validTransactions.length > 0 && (
                            <div className="preview-table-container">
                                <h3>Valid Transactions ({preview.validTransactions.length})</h3>
                                <table className="preview-table">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Date</th>
                                            <th>Description</th>
                                            <th>Amount</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {preview.validTransactions.map((t, i) => (
                                            <tr key={i}>
                                                <td>{i + 1}</td>
                                                <td>{new Date(t.date).toLocaleDateString()}</td>
                                                <td>{t.description}</td>
                                                <td className={t.amount >= 0 ? "amount-positive" : "amount-negative"}>
                                                    {t.amount >= 0 ? "+" : ""}${Math.abs(t.amount).toFixed(2)}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}

                        {preview.errors.length > 0 && (
                            <div className="preview-table-container">
                                <h3 className="error-heading">Errors ({preview.errors.length})</h3>
                                <table className="preview-table error-table">
                                    <thead>
                                        <tr>
                                            <th>Row</th>
                                            <th>Error</th>
                                            <th>Data</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {preview.errors.map((e, i) => (
                                            <tr key={i}>
                                                <td>{e.rowNumbers}</td>
                                                <td className="error-cell">{e.errors}</td>
                                                <td className="mono-cell">{e.rowData}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}

                        {preview.successCount > 0 && accountId && (
                            <div className="confirm-section">
                                <button
                                    onClick={handleConfirmImport}
                                    disabled={confirming}
                                    className="btn-confirm"
                                >
                                    {confirming
                                        ? "Importing..."
                                        : `Confirm Import (${preview.successCount} transactions)`}
                                </button>
                                <p className="confirm-hint">
                                    ⚠️ This will add {preview.successCount} transactions to your account
                                </p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
        </>

    )
    
}