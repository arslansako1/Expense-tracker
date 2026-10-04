import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/AutoPaymentsPage/AutoPaymentsButtons.css";


export default function DeleteAutoPaymentPopup({autoPayment, onClose}){
    const dialogRef = useRef(null);
    const [autoPaymentId, setAutoPaymentId] = useState(0);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();
        setAutoPaymentId(autoPayment.id);

    }, [])


    const handleDeleteAutoPayment = async (e) => {
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);

   
        try{
            const response = await fetch(API_ENDPOINTS.deleteRecurringRule(autoPaymentId),{
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to delete auto payment");
                throw new Error("Failed to delete auto payment");
            }

            onClose();


        } catch(error){
            setError("Error deleting auto payment");
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog className="deleteAutoPayment-dialog" ref={dialogRef}>
      <button className="close-autoPayment" onClick={() => onClose()}>✕</button>
        <p className="delete-text">Are you sure you want to delete this auto payment?</p>
      <button className="yes-autoPayment" onClick={() => handleDeleteAutoPayment()}>Yes</button>
    </dialog>
  );
    
}