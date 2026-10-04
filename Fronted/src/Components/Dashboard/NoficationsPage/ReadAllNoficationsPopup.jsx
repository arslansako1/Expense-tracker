import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import Nofications from "./Nofications";


export default function ReadAllNoficationPopup({onClose}){
    const dialogRef = useRef(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();

    }, [])

    const handleReadAllNotifications = async (e) => {
        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);


        
        try{
            const response = await fetch(API_ENDPOINTS.markAllAsReadNofication,{
                method: "PUT",
                headers: {
                    "Authorization": "Bearer " + token,
                },

            });

            if (!response.ok){
                
                setError("Failed to read all nofication"); 
                throw new Error("Failed to read all nofication");
            }

            

            onClose();


        } catch(error){
            setError("Error reading all nofication: ", error);
            

        } finally{
            setLoading(false);
        }

    }

    return(
    <dialog ref={dialogRef}>
        <p>Are you sure you want to mark all as read?</p>
      <button onClick={() => handleReadAllNotifications()}>Yes</button>
      <button onClick={() => onClose()}>Close</button>
    </dialog>
  );
    
}