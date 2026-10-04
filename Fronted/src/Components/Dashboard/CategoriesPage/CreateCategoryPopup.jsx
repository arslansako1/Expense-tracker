import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useViewTransitionState } from "react-router-dom";


export default function CreateCategoryPopup({onClose}){
    const dialogRef = useRef(null);
    const [name, setName] = useState("");
    const [icon, setIcon] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        dialogRef.current.showModal();


    }, [])


    const handleCreateCategory = async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setError(null);

        if (!name){
            setError("fill in the missing requirements");
            return;
        }

        const createCategory = {
            Name: name,
            Icon: icon
        };

        setLoading(true);


        try{
            var response = await fetch(API_ENDPOINTS.createCategory,{
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify(createCategory)
            });

            if (!response.ok){
                
                setError("Failed to create category");
                throw new Error("Failed to create category");
            }

            const data = await response.json();
            

            
            onClose();


        } catch(error){
            setError("Error creating category");
            

        } finally{
            setLoading(false);
        }

    }


    return(
        <dialog className="createCategory-dialog" ref={dialogRef}>
            <button className="create-close-btn" onClick={() => onClose()}>✕</button>

            <form onSubmit={handleCreateCategory}>
             {error && <div className="create-budget-error">{error}</div>}

                <label>Name</label>
                <input autoFocus type="text" value={name} onChange={(n) => setName(n.target.value)}/>

                <label>Icon</label>
                <input autoFocus type="text" value={icon} onChange={(i) => setIcon(i.target.value)}/>

            <button className="create-form-button" type="submit">{loading ? "Creating..." : "Create"}</button>
            </form>

        </dialog>

    )
}