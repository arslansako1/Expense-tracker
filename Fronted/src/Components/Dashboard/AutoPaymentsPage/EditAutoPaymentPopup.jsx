import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AutoPaymentsPage/AutoPaymentsButtons.css";


export default function EditAutoPaymentPopup({accounts, categories, autoPayment, onClose}){
    const dialogRef = useRef(null);
    const [amount, setAmount] = useState(autoPayment.amount);
    const [frequency, setFrequency] = useState(autoPayment.frequency);
    const [nextRunDate, setNextRunDate] = useState(new Date().toISOString().split("T")[0]);
    const [accountId, setAccountId] = useState(autoPayment.accountId);
    const [allAccounts, setAccounts] = useState(accounts);
    const [categoryId, setCategoryId] = useState(autoPayment.categoryId);
    const [allCategories, setAllCategories] = useState(categories);
    const [autoPaymentId, setAutoPaymentId] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        setAutoPaymentId(autoPayment.id);
        setAccountId(autoPayment.accountId);
        setCategoryId(autoPayment.categoryId)
        setAmount(autoPayment.amount);
        setFrequency(autoPayment.frequency);

        if (autoPayment.nextRunDate){
          const formattedDate = new Date(autoPayment.nextRunDate).toISOString().split("T")[0];
          setNextRunDate(formattedDate);
        }

    }, [])


    const handleEditAutoPayment = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!accountId || !categoryId || !amount || !frequency || !nextRunDate){
          setError("Fill in the requirements");
          return;
        }

        const editAutoPayment = {
            AccountId: accountId,
            CategoryId: categoryId,
            Amount: amount,
            Frequency: frequency,
            NextRunDate: new Date(nextRunDate).toISOString()
        };

        setLoading(true);

        try{
            var response = await fetch(API_ENDPOINTS.updateRecurringRule(autoPaymentId),{
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(editAutoPayment)
            });

            if (!response.ok){
                
                setError("Failed to edit auto payment");
                throw new Error("Failed to edit auto payment");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error editing auto payment", error);
            

        } finally{
            setLoading(false);
        }

    }



    return(
    <dialog className="editAutoPayment-dialog" ref={dialogRef}>
      
        {error && <div className="autoPayment-error">{error}</div>}

      <button className="closeAutoPayment" onClick={() => onClose()}>✕</button>

      <form onSubmit={handleEditAutoPayment}>

              <label className="accountLabel">Account</label>
                <select value={accountId} onChange={(c) => setAccountId(parseInt(c.target.value))}>
                    <option value="">Select an account</option> 
                    {allAccounts.map((account) => (
                        <option value={account.id} key={account.id}>{`${account.name}`}</option>
                    ))}     
                </select>

           <label className="amountLabel">Amount</label>
                <input autoFocus className="amountInput" type="number" value={amount} onChange={(a) => setAmount(parseFloat(a.target.value))}/>

                <label className="frequencyLabel">Frequency</label>
                <select value={frequency} onChange={(f) => setFrequency(f.target.value)}>
                <option value={"Daily"}>Daily</option>
                <option value={"Weekly"}>Weekly</option>
                <option value={"BiWeekly"}>Bi-Weekly</option>
                <option value={"Monthly"}>Monthly</option>
                <option value={"Quarterly"}>Quarterly</option>
                <option value={"Yearly"}>Yearly</option>
                </select>

                <label className="nextRunDateLabel">When should this start?</label>
                <input autoFocus className="nextRunDateInput" type="date" value={nextRunDate} onChange={(d) => setNextRunDate(d.target.value)}/>

         <label className="categoryLabel">Category</label>
                <select value={categoryId} onChange={(c) => setCategoryId(parseInt(c.target.value))}>
                    <option value="">Select a category</option> 
                    {allCategories.map((category) => (
                        <option value={category.id} key={category.id}>{`${category.name} ${category.icon}`}</option>
                    ))}     
                </select>

        <button className="transaction-create-button" type="submit">{loading ? "Edting..." : "Edit"}</button>
      </form>

    </dialog>
  );
    
}