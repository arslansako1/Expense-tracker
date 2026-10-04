import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AutoPaymentsPage/AutoPaymentsButtons.css";


export default function CreateAutoPaymentPopup({onClose}){
    const dialogRef = useRef(null);
    const [amount, setAmount] = useState(0);
    const [frequency, setFrequency] = useState("Daily");
    const [nextRunDate, setNextRunDate] = useState(new Date().toISOString().split("T")[0]);
    const [categoryId, setCategoryId] = useState(0);
    const [categories, setCategories] = useState([]);
    const [accountId, setAccountId] = useState();
    const [accounts, setAccounts] = useState([]);
    const [loading, setLoading] = useState(false);
    const [loadingCategory, setLoadingCategory] = useState(false);
    const [loadingAccount, setLoadingAccount] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        fetchAllCategories();
        fetchAllAccounts();

    }, [])


    const handleCreateAutoPayment = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!accountId || !categoryId || !amount || !frequency || !nextRunDate){
          setError("fill in the requirements");
          return;
        }

        const createAutoPayment = {
            AccountId: accountId,
            CategoryId: categoryId,
            Amount: amount,
            Frequency: frequency,
            NextRunDate: new Date(nextRunDate).toISOString()
        };

        setLoading(true);

        try{
            var response = await fetch(API_ENDPOINTS.createRecurringRule,{
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(createAutoPayment)
            });

            if (!response.ok){
                
                setError("Failed to create auto payment");
                throw new Error("Failed to create auto payment");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error creating auto payment", error);
            

        } finally{
            setLoading(false);
        }

    }
    
        const fetchAllCategories = async (e) => {
            
            const token = localStorage.getItem("token");
            if (!token){
              
              setLoading(false);
              return;
            }
      
          setLoadingCategory(true);
          setError(null);
      
          
      
          try{
            
            const response = await fetch(API_ENDPOINTS.getAllCategories,{
              headers: {
                Authorization: "Bearer " + token, 
              },
            });
      
            if (!response.ok){
              
              setError("Failed to fetch all categories");
              throw new Error("Failed to fetch all categories");
            }
      
            const data = await response.json();
            
            setCategories(data);
      
          } catch(error){
            
            setError("Error fetching all categories");
      
          } finally{
           setLoadingCategory(false);
          }
        }

        const fetchAllAccounts = async (e) => {
            
            const token = localStorage.getItem("token");
            if (!token){
              
              setLoading(false);
              return;
            }
      
          setLoadingAccount(true);
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
            
            setAccounts(data);
      
          } catch(error){
            
            setError("Error fetching all Accounts");
      
          } finally{
           setLoadingAccount(false);
          }
        }



    return(
        <dialog className="autoPayment-dialog" ref={dialogRef}>

        {error && <div className="autoPayment-error">{error}</div>}

            <button className="closeAutoPayment" onClick={() => onClose()}>✕</button>
          
            <form onSubmit={handleCreateAutoPayment}>
              
                <label className="accountLabel">Account</label>
                <select value={accountId} onChange={(c) => setAccountId(parseInt(c.target.value))}>
                    <option value="">Select an account</option> 
                    {accounts.map((account) => (
                        <option value={account.id} key={account.id}>{`${account.name}`}</option>
                    ))}     
                </select>

                <label className="amountLabel">Amount</label>
                <input autoFocus className="amountInput" type="number" value={amount} onChange={(a) => setAmount(parseFloat(a.target.value))}/>

                <label className="freqeuncyLabel">Frequency</label>
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

                <label className="categoryLabel" >Category</label>
                <select value={categoryId} onChange={(c) => setCategoryId(parseInt(c.target.value))}>
                    <option value="">Select a category</option> 
                    {categories.map((category) => (
                        <option value={category.id} key={category.id}>{`${category.name} ${category.icon}`}</option>
                    ))}     
                </select>

            <button className="transaction-create-button" type="submit">{loading ? "Creating..." : "Create"}</button>
            </form>

        </dialog>

    )
}