import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AccountPage/CreateTransferPopup.css";


export default function CreateTransferPopup({account, onClose}){
    const dialogRef = useRef(null);
    const [amount, setAmount] = useState(0);
    const [description, setDescription] = useState("");
    const [fromAccountId, setFromAccountId] = useState(0);
    const [toAccountId, setToAccountId] = useState(0);
    const [accounts, setAccounts] = useState([]);
    const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
    const [loading, setLoading] = useState(false);
    const [loadingAccounts, setLoadingAccounts] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        fetchAllAccounts();
        if (account && account.id){
        setFromAccountId(account.id);
        }

    }, [account])


   const handleCreateTransfer = async (e) => {
    e.preventDefault();

    const token = localStorage.getItem("token");
    if (!token) {
        setLoading(false);
        setError("User must be logged in");
        return;
    }

        if (!amount || !toAccountId || !description){
           setError("Fill the missing requirements");
          return;
        }

    setError(null);

    const createTransfer = {
        Amount: parseFloat(amount),
        Description: description,
        FromAccountId: parseInt(fromAccountId),
        ToAccountId: parseInt(toAccountId),
        Date: new Date(date).toISOString()
    };

    setLoading(true);

    try {
        const response = await fetch(API_ENDPOINTS.createTransfer, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
            body: JSON.stringify(createTransfer)
        });

        if (!response.ok) {
            const errorTxt = await response.text();
            setError(errorTxt || "Failed to create transfer");
            throw new Error(errorTxt || "Failed to create transfer");
        }

        const data = await response.json();

        onClose();

    } catch (error) {
        setError(`Error creating transfer: ${error.message}`);
        
    } finally {
        setLoading(false);
    }
};

    const fetchAllAccounts = async (e) => {
      const token = localStorage.getItem("token");
      if (!token){
        setLoading(false);
        return;
      }

    setLoadingAccounts(true);
    setError(null);


    try{
      const response = await fetch(API_ENDPOINTS.getAllAccounts,{
        headers: {
          Authorization: "Bearer " + token, 
        },
      });

      if (!response.ok){
        setError("Failed to fetch all accounts");
        throw new Error("Failed to fetch all accounts");
      }

      const data = await response.json();

      const sortedAccounts = data.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
      setAccounts(sortedAccounts);

    } catch(error){
      setError(`Error fetching all accounts: ${error.message}`);

    } finally{
     setLoadingAccounts(false);
    }
  }



    return(
        <dialog className="transfer-dialog" ref={dialogRef}>
            <button className="transfer-close-button" onClick={() => onClose()}>✕</button>

            <form onSubmit={handleCreateTransfer}>

           {error && <div className="transfer-error">{error}</div>}


                <label className="transfer-amount">Amount</label>
                <input autoFocus type="number" value={amount} onChange={(a) => setAmount(a.target.value)}/>

                <label className="transfer-description">description</label>
                <input autoFocus type="text" value={description} onChange={(d) => setDescription(d.target.value)}/>

                <label className="transfer-toAccount">To account</label>
                <select value={toAccountId} onChange={(a) => setToAccountId(parseInt(a.target.value))}>
                    <option value="">Select an account</option> 
                    {accounts.filter((account) => account.id !== fromAccountId).map((account) => (
                        <option value={account.id} key={account.id}>{`${account.name}`}</option>
                    ))}     
                </select>

            <button className="transfer-create-button" type="submit">{loading ? "transfering..." : "transfer"}</button>
            </form>

        </dialog>

    )
}