import { Parser } from "xml2js";

const APIurl = "https://vkjqon2du0.execute-api.us-east-1.amazonaws.com";
const parser = new Parser()


export const consumeSQS = async (resFunc) => {
    try {
        // Perform request query with 20 second timeout
        var message = await fetch(`${APIurl}/nyToppscore`);
    } catch (err) {
        console.log("failed to fetch:", err);
        resFunc(null);
        return;
    }
        
    // Received message in XML from SQS via API Gateway
    let text = await message.text();
    parser.parseString(text, function(err, res) {
        if (err === null) {
            let messageResults = res.ReceiveMessageResponse.ReceiveMessageResult
            if (!messageResults[0]) {
                resFunc(null);
                return;
            }

            messageResults.forEach(lst => {
                lst.Message.forEach(item => {
                    resFunc(item.Body[0]);
                    fetch(`${APIurl}/nyToppscore`, {
                        method: "DELETE",
                        body: JSON.stringify({handle: item.ReceiptHandle[0]})
                    })
                })
            })
        }
        else console.error(err)
    })
}

export const getTop6 = async (resFunc) => {
    let res = await fetch(`${APIurl}/nisseekspress/topp6`);
    let json = await res.json();
    resFunc(json);
}